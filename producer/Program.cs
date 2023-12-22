using Producer.Service;
namespace producer;

class Program
{
    static async Task Main(string[] args)
    {
        var cancellationToken = new CancellationToken();
        try{

            await  ProducerService.ProduceAsync(cancellationToken);
        }
        catch(Exception ex){
            Console.WriteLine(ex.Message);
        }
      
        Console.WriteLine("Hello, World!");
        
    }
}
