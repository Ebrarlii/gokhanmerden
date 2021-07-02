using System.Collections.Generic;
using System.Threading.Tasks;
using Ishop.Core.Finance.Entity;

namespace Ishop.Core.Finance.Services
{
    //////////////////////////
    // Tahakkuk Onaylama Arayüzü
    //////////////////////////
    public interface IAccrueApprovementServices {
           List<ApproveAccrueResultModel> ApproveAccrue(List<ApproveAccrueRequestModel> model);
           List<ApproveAccrueResultModel> readExcelFile(ExcelFileModel model);
    }
}