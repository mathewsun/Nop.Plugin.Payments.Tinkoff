﻿using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Enums;

namespace Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.ResponseEntity
{
    public class Submit3DSAuthorizationResponse : TinkoffResponse
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string RebillId { get; set; }
        public string CardId { get; set; }

        public EStatusResponse Status { get; set; }
    }
}