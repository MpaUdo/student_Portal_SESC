namespace StudentPortal.Models.Dtos.Requests;

public class GenerateInvoiceDto {

    public decimal Amount { get; set; }
    public string DueDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    public string Type { get; set; } = "TUITION_FEES";
    public AccountDto Account{ get; set; }
}