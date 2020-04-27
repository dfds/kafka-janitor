using System;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaJanitor.IntegrationTests.Utils
{
    public static class DoUntil
    {
        public static TResult ResultOrTimespan<TResult>(
            Func<TResult> function, 
            TimeSpan deadline
        )
        {
            var endTime = DateTime.Now.Add(deadline);
            dynamic result;
            do
            {
                result = function();
                if (result == null)
                {
                    Thread.Sleep(1000);
                }
            } while (result == null && DateTime.Now < endTime);

            return result;
        }
        
        
        public static async Task<TResult> ResultOrTimespanAsync<TResult>(
            Func<Task<TResult>> function,
            TimeSpan deadline
        )
        {
            var endTime = DateTime.Now.Add(deadline);
            dynamic result;
            do
            {
                result = await function();
                if (result == null)
                {
                    Thread.Sleep(1000);
                }
            } while (result == null && DateTime.Now < endTime);

            return result;
        }
    }
    
    
}