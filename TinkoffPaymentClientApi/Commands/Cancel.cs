﻿using System;

using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Attributes;
using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Models;

namespace Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Commands
{
    public class Cancel : BaseCommand
    {
        public string PaymentId { get; private set; }
        public decimal? Amount { get; set; }
        public string IP { get; set; }

        [IgnoreTokenCalculate]
        public Receipt Receipt { get; set; }

        internal override string CommandName => "Cancel";

        public Cancel(string paymentId)
        {
            if (string.IsNullOrEmpty(paymentId))
            {
                throw new ArgumentNullException(nameof(paymentId), "Must be not empty");
            }
            PaymentId = paymentId;
        }
    }
}