using System.Threading.Tasks;
using Gokhan.Core.Services;
using Ishop.Core.Finance.Data;
using Ishop.Core.Finance.Entity;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using Ishop.Core.Finance.Data;
using System;
using OfficeOpenXml;

namespace Ishop.Core.Finance.Services
{
    //////////////////////
    //  Tahakkuk onaylama servisi
    //////////////////////
    public class AccrueApprovementServices : IAccrueApprovementServices
    {
        #region consts
            const int SYSTEM_VOUCHER_NO = 1;
            const int CONFIRM_NO = 2;
            const int CONFIRM_DATE = 3;
        #endregion

        FinanceUnitOfWork _financeUnitOfWork;
        public AccrueApprovementServices(IConfiguration config, FinanceAppSettings financeAppSettings){
            _financeUnitOfWork = new FinanceUnitOfWork(config);
        }

        public List<ApproveAccrueResultModel> ApproveAccrue(List<ApproveAccrueRequestModel> modelList)
        {

            List<ApproveAccrueResultModel> resultList = new List<ApproveAccrueResultModel>();
            foreach(var model in modelList){
                PaymentVoucher paymentVoucher = _financeUnitOfWork.PaymentVoucherRepository
                .Get(p=>p.voucherNo == model.SystemVoucherNo);
                Voucher voucher = _financeUnitOfWork.VoucherRepository.Get(p=> p.voucherNo == model.SystemVoucherNo && p.rowNo == 0);
                PaymentSummary ps = _financeUnitOfWork.PaymentSummaryRepository.Get(p=> p.voucherNo == model.SystemVoucherNo);
            
                var validateResult = ValidatePaymentConfirm(paymentVoucher,voucher,ps);
                if (!validateResult){
                    continue;
                }
                
                decimal debitAmount = paymentVoucher.accrumentAmount - (ps != null ? ps.debitAmount : 0m);
                if (debitAmount > 0m) {

                        Payment payment = _financeUnitOfWork.PaymentRepository.Get(p=>p.voucherNo == paymentVoucher.voucherNo);
                        if (payment == null){
                            payment = new Payment();
                        }
                        payment.voucherNo = paymentVoucher.voucherNo;
                        payment.amountType = "D";
                        payment.debitAmount = debitAmount;
                        payment.creditAmount = 0m;
                        payment.confirmNo = model.ConfirmNo;
                        payment.confirmDate = model.ConfirmDate;
                        payment.createdUserId = 1111111;
                        payment.createdDate = DateTime.Now;
                        if (payment.id == 0) {
                            _financeUnitOfWork.PaymentRepository.Insert(payment);
                        } else {
                            _financeUnitOfWork.PaymentRepository.Update(payment);
                        }
                        voucher.voucherStatus = VoucherStatus.Confirmed;
                        //ModifiedUtils.ApplyModifiedInfo(voucher, Isuzem.GelirGider.X.EtkinKullanici.ID);
                        _financeUnitOfWork.VoucherRepository.Update(voucher);
                        paymentVoucher.confirmOrderNo = model.ConfirmNo;
                        paymentVoucher.confirmDate = model.ConfirmDate;
                        paymentVoucher.accrumentStatus = AccruementStatus.Confirm;
                        _financeUnitOfWork.PaymentVoucherRepository.Update(paymentVoucher);
                        Ishop.Core.Finance.Entity.ApproveAccrueResultModel resultModel = new ApproveAccrueResultModel();
                        resultModel.SystemVoucherNo = model.SystemVoucherNo; 
                        resultModel.Message = "Kayıt onaylandı";
                        resultList.Add(resultModel);
                }
                           
            }
            _financeUnitOfWork.Save();
            return resultList;
        }

        public List<ApproveAccrueResultModel> readExcelFile(ExcelFileModel model)
        {
            List<InvalidExcelException> _exceptionList = new List<InvalidExcelException>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using(ExcelPackage package = new ExcelPackage(model.file)) {
                if (package == null) {
                    InvalidExcelException exception = new InvalidExcelException("Geçersiz excel formartı");
                    _exceptionList.Add(exception);
                }

                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                List<ApproveAccrueRequestModel> accrueModels = new List<ApproveAccrueRequestModel>();
                for(int row=2;row<=rowCount;row++) {
                    object obj_system_voucher_no = worksheet.Cells[row,SYSTEM_VOUCHER_NO].Value;
                    object obj_confirm_no = worksheet.Cells[row,CONFIRM_NO].Value;
                    object obj_confirm_date = worksheet.Cells[row,CONFIRM_DATE].Value;
                    if (Convert.ToString(obj_system_voucher_no).Length==0) {
                        continue;
                    }
                    int voucherNo = 0;
                    int confirmNo = 0;
                    if (int.TryParse(Convert.ToString(obj_system_voucher_no),out voucherNo)) {
                        if (int.TryParse(Convert.ToString(obj_confirm_no),out confirmNo)) {
                            DateTime confirmDate = (DateTime)obj_confirm_date; //DateTime.FromOADate(Convert.ToDouble(obj_confirm_date));
                            ApproveAccrueRequestModel accrueModel = new ApproveAccrueRequestModel();
                            accrueModel.SystemVoucherNo = voucherNo;
                            accrueModel.ConfirmNo = confirmNo;
                            accrueModel.ConfirmDate = confirmDate;
                            accrueModels.Add(accrueModel);
                        }
                    }
                }
                List<ApproveAccrueResultModel> accrueResultModels = ApproveAccrue(accrueModels);

                return accrueResultModels;
            }
        }
        private bool ValidatePaymentConfirm(PaymentVoucher pv,Voucher voucher,PaymentSummary ps) {

            // tf.Transaction != null ? new V300PaymentSummary(tf.Transaction, tf.VoucherNo) : 
            // new V300PaymentSummary(tf.VoucherNo);
            if (ps != null && ps.debitAmount >= pv.accrumentAmount) {
                return false;
            }
            
            if (pv.accrumentStatus == AccruementStatus.Confirm) {
                return false;
            }
            /* if (pv.accrumentStatus == AccruementStatus.Cancel) {
                return false;
            } */
            // voucher = tf.Transaction != null ? new T300Voucher(tf.Transaction, tf.VoucherNo, 0) : 
            // new T300Voucher(tf.VoucherNo, 0);
            if (voucher != null && (voucher.isCancelled || voucher.voucherStatus != VoucherStatus.Enabled)) {
                return false;
            }
            return true;
        }
    }
}