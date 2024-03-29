﻿using System;

namespace Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Commands
{
    public class GetCustomer : BaseCommand
    {
        public string CustomerKey { get; private set; }
        public string IP { get; set; }

        internal override string CommandName => "GetCustomer";

        public GetCustomer(string customerKey)
        {
            if (string.IsNullOrEmpty(customerKey))
            {
                throw new ArgumentNullException(nameof(customerKey), "Must be not empty");
            }
            CustomerKey = customerKey;
        }
    }
}