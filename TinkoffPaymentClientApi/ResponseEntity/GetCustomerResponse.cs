using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.ResponseEntity;

namespace TinkoffPaymentClientApi.ResponseEntity
{
    public class GetCustomerResponse : CustomerResponse {
    public string Email { get; set; }
    public string Phone { get; set; }
  }
}