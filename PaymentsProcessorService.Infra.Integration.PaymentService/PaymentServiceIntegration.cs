using Payments.Domain.Models;

namespace PaymentsProcessorService.Infra.Integration.PaymentService;
public class PaymentServiceIntegration
{

    public PaymentServiceIntegration()
    {
    }

    public PaymentResult ProcessPayment(PaymentCreatedEvent eventCreated)
    {
        var rand = new Random();
        if (rand.Next(1, 11) < 9)
        {
            return new PaymentResult
            {
                Success = true,
                Message = "Pagamento efetuado com sucesso"
            };
        }

        return new PaymentResult
        {
            Success = false,
            Message = "Falha ao processar o pagamento"
        };
    }

}
public class PaymentResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

