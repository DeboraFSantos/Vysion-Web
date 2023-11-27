using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vysion.Entities;

namespace Vysion.Repositories
{
    public interface IPaymentMethodsRepository
    {
        Task<PaymentMethod> GetPaymentMethod(Guid id);
        IEnumerable<PaymentMethod> GetPaymentMethods();

        void CreatePaymentMethod(PaymentMethod paymentMethod);
        void UpdatePaymentMethod (PaymentMethod paymentMethod);
        void DeletePaymentMethod (Guid id);
    }
}