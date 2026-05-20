using System.IO;
using Heven.Api.Application.Bookings.Commands.ConfirmPayment;
using Heven.Api.Web.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.V2.Core;

namespace Heven.Api.Web.Endpoints;

public class Webhook : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            // Nhận POST request từ Stripe tại /api/webhook/stripe 
            .MapPost(StripeWebhook, "stripe"); 
    }

    public async Task<IResult> StripeWebhook(
        HttpContext context,
        ISender sender,
        IConfiguration configuration)
    {
        var json = await new StreamReader(context.Request.Body).ReadToEndAsync();

        try
        {
            // 2. Xác thực chữ ký bằng WebhookSecret
            var stripeSignature = context.Request.Headers["Stripe-Signature"];
            var webhookSecret = configuration.GetSection("StripeSettings")["WebhookSecret"];

            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                webhookSecret
            );

            // 3. Kiểm tra loại sự kiện là Checkout Session Cơmpleted (Thanh toán hoàn tất)
            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
            {
                var session = stripeEvent.Data.Object as Session;

                // 4. Lấy lại ClientReferenceId (Là BookingId)
                if (session != null && int.TryParse(session.ClientReferenceId, out int bookingId))
                {
                    await sender.Send(new ConfirmPaymentCommand(bookingId, session.Id));
                }
            }

            return Results.Ok();
        }
        catch (StripeException e)
        {
            // Nếu sai chữ ký (fake request) hoặc lỗi từ Stripe, trả về 400
            return Results.BadRequest(e.Message);
        }
    }
}
