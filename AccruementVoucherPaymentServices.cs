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
    //  Tahakkuk ödeme işlemleri servisi
    //////////////////////
    public class AccruementVoucherPaymentServices : IAccruementVoucherPaymentServices
    {
        FinanceUnitOfWork _financeUnitOfWork;
        SequenceServices _sequenceServices;

        #region consts
            const int SYSTEM_VOUCHER_NO = 1;
            const int PAYMENT_NO = 2;
            const int PAYMENT_DATE = 3;
            const int PAYMENT_AMOUNT = 4;
        #endregion
        public AccruementVoucherPaymentServices(IConfiguration config, FinanceAppSettings financeAppSettings){
            _financeUnitOfWork = new FinanceUnitOfWork(config);
            _sequenceServices = new SequenceServices(config);
        }

        public List<PayVoucherResultModel> PayVoucher(List<PayVoucherRequestModel> model)
        {
            List<PayVoucherResultModel> resultList = new List<PayVoucherResultModel>();
            foreach (var requestModel in model)
            {
                PaymentVoucher paymentVoucher = _financeUnitOfWork.PaymentVoucherRepository
                .Get(p=>p.voucherNo == requestModel.SystemVoucherNo);
                Voucher voucher = _financeUnitOfWork.VoucherRepository.Get(p=>p.voucherNo == requestModel.SystemVoucherNo && p.rowNo == 0);
                PaymentSummary paymentSummary = _financeUnitOfWork.PaymentSummaryRepository.Get(p=>p.voucherNo == requestModel.SystemVoucherNo);
                var result = AccruementVoucherPaymentControl(paymentVoucher,requestModel.PaymentAmount,voucher,paymentSummary);
                if (!result) {
                    continue;
                }
                voucher.voucherStatus = VoucherStatus.Paid;
                _financeUnitOfWork.VoucherRepository.Update(voucher);

                double paymentVoucherNo = _sequenceServices.getNextValue("T300VOUCHER");
                paymentVoucher.paymentVoucherNo = Convert.ToInt32(paymentVoucherNo);
                paymentVoucher.paymentNo = requestModel.PaymentNo;
                paymentVoucher.paymentDate = requestModel.PaymentDate;
                paymentVoucher.paymentAmount = requestModel.PaymentAmount;
                paymentVoucher.accrumentStatus = AccruementStatus.Payment;

                _financeUnitOfWork.PaymentVoucherRepository.Update(paymentVoucher);

                Voucher voucherp = new Voucher();
                voucherp.voucherNo = Convert.ToInt32(paymentVoucherNo);
                voucherp.rowNo = 0;
                voucherp.rowDate = DateTime.Now;
                voucherp.unitNo = paymentVoucher.unitNo;
                voucherp.relatedUnitNo = paymentVoucher.unitNo;
                voucherp.accountNo = voucher.accountNo;
                voucherp.yearNo = (short)(paymentVoucher.paymentDate?.Year);
                voucherp.monthNo = (byte)(paymentVoucher.paymentDate?.Month);
                voucherp.distribution = DistributionStatus.CashPayment;
                voucherp.voucherType = 8;
                voucherp.voucherStatus = VoucherStatus.Enabled;
                voucherp.prodOrCancel = "O";
                voucherp.isCancelled = false;
                voucherp.grossAmount = requestModel.PaymentAmount;
                voucherp.kdvRate = voucher.kdvRate;
                voucherp.kdvAmount = voucherp.grossAmount * voucherp.kdvRate;
                voucherp.netAmount = voucherp.grossAmount - voucherp.kdvAmount;

                _financeUnitOfWork.VoucherRepository.Insert(voucherp);

                Payment payment = new Payment();
                payment.voucherNo = paymentVoucher.voucherNo;
                payment.amountType = "C";
                payment.debitAmount = 0m;
                payment.creditAmount = requestModel.PaymentAmount; // paymentAmount; // voucher.GrossAmount;
                payment.paymentVoucherNo = (int)paymentVoucher.paymentVoucherNo;
                payment.paymentNo = requestModel.PaymentNo; // paymentNo;
                payment.paymentDate = requestModel.PaymentDate; // paymentDate;
                payment.createdUserId = 123456;//Isuzem.GelirGider.EtkinKullanici.Current.ID;
                payment.createdDate = DateTime.Now;

                _financeUnitOfWork.PaymentRepository.Insert(payment);
                PayVoucherResultModel resultModel = new PayVoucherResultModel();
                resultModel.SystemVoucherNo = requestModel.SystemVoucherNo;
                resultModel.Message = "Ödeme Yapıldı";
                resultList.Add(resultModel);    
            }
            _financeUnitOfWork.Save();

            return resultList;
        }

        public List<PayVoucherResultModel> readExcelFile(ExcelFileModel model)
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
                List<Entity.PayVoucherRequestModel> payVoucherModels = new List<PayVoucherRequestModel>();
                for(int row=2;row<=rowCount;row++) {
                    object obj_system_voucher_no = worksheet.Cells[row,SYSTEM_VOUCHER_NO].Value;
                    object obj_payment_no = worksheet.Cells[row,PAYMENT_NO].Value;
                    object obj_payment_date = worksheet.Cells[row,PAYMENT_DATE].Value;
                    object obj_payment_amount = worksheet.Cells[row,PAYMENT_AMOUNT].Value;
                    if (Convert.ToString(obj_system_voucher_no).Length==0) {
                        continue;
                    }
                    int voucherNo = 0;
                    int paymentNo = 0;
                    decimal paymentAmount = 0;
                    if (int.TryParse(Convert.ToString(obj_system_voucher_no),out voucherNo)) {
                        if (int.TryParse(Convert.ToString(obj_payment_no),out paymentNo)) {
                            if (decimal.TryParse(Convert.ToString(obj_payment_amount),out paymentAmount)) {
                                DateTime paymentDate = (DateTime)obj_payment_date;
                                PayVoucherRequestModel payVoucherModel = new PayVoucherRequestModel();
                                payVoucherModel.SystemVoucherNo = voucherNo;
                                payVoucherModel.PaymentNo = paymentNo;
                                payVoucherModel.PaymentDate = paymentDate;
                                payVoucherModel.PaymentAmount = paymentAmount;
                                payVoucherModels.Add(payVoucherModel);
                            }
                        }
                    }
                }
                List<PayVoucherResultModel> payVoucherResultModels = PayVoucher(payVoucherModels);

                return payVoucherResultModels;
            }
        }

        private bool AccruementVoucherPaymentControl(PaymentVoucher paymentVoucher,decimal paymentAmount,
                                                     Voucher voucher,PaymentSummary paymentSummary){
            if (paymentAmount <= 0m){
                return false;
            }
            if (paymentVoucher == null){
                return false;
            }
            if (paymentSummary != null) {
                if (paymentSummary.debitAmount <= 0m) {
                    return false;
                }

                if (paymentSummary.balance <= 0m) {
                    return false;
                }

                if (paymentSummary.balance +1 < paymentAmount) {
                    return false;
                }
            } else {
                return false;
            }
            if (voucher == null)
                return false;
            if (voucher.isCancelled ) {
                return false;
            }
            return true;
        }

    }

}