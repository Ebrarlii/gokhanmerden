using System;

namespace Ishop.Core.Finance.Services
{
    public class InvalidExcelException : Exception
    {
        public InvalidExcelException()
        {
        }

        public InvalidExcelException(string message)
            : base(message)
        {
        }

        public InvalidExcelException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}