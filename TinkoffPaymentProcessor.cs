using Microsoft.AspNetCore.Http;

using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi;
using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Commands;
using Nop.Plugin.Payments.Tinkoff.TinkoffPaymentClientApi.Models;
using Nop.Services.Catalog;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nop.Plugin.Payments.Tinkoff
{
    public class TinkoffPaymentProcessor : BasePlugin, IPaymentMethod
    {
        private readonly IWebHelper _webHelper;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TinkoffPaymentProcessor(
            IWebHelper webHelper,
            IOrderService orderService,
            IProductService productService,
            IHttpContextAccessor httpContextAccessor)
        {
            _webHelper = webHelper;
            _orderService = orderService;
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/TinkoffPayment/Configure";
        }

        //Copied from PayPalStandart plugin
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return false;

            return true;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 5;
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        public string GetPublicViewComponentName()
        {
            return "TinkoffPayment";
        }

        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            return false;
        }

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            using (var clientApi = new TinkoffPaymentClient("1616877814849DEMO", "ji8fz54h4pw8od4u"))
            {
                CancellationToken cancellationToken = CancellationToken.None;

                var result = clientApi.InitAsync(
                    new Init(postProcessPaymentRequest.Order.Id.ToString(),
                             postProcessPaymentRequest.Order.OrderTotal)
                    {
                        Receipt = new Receipt("alex.pigaloyv@gmail.com", TinkoffPaymentClientApi.Enums.ETaxation.osn)
                        {
                            ReceiptItems = _orderService.GetOrderItems(postProcessPaymentRequest.Order.Id).Select(x =>
                            {
                                var product = _productService.GetProductById(x.ProductId);

                                var item = new ReceiptItem(
                                    product.Name,
                                    x.Quantity,
                                    product.Price,
                                    TinkoffPaymentClientApi.Enums.ETax.vat20);

                                return item;
                            })
                        },
                        Data = new Dictionary<string, string>()
                        {

                        }
                    }, cancellationToken).Result;

                if (result.Success)
                {
                    _httpContextAccessor.HttpContext.Response.Redirect(result.PaymentURL);
                }
            }
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult();
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        #region Properties

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;

        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => true;

        public string PaymentMethodDescription => "";

        #endregion
    }
}
