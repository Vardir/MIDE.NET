namespace Vardirsoft.XApp
{
    public class OperationResult
    {
        private readonly Success _success;
        private readonly Failure _failure;

        public static OperationResult SuccessfulResult() => new OperationResult(new Success());
        public static OperationResult FailWith(string message) => new OperationResult(new Failure(message));
        
        public bool IsSuccess => _success != null;
        public bool IsFailure => _failure != null;
        
        protected OperationResult()
        {
            
        }
        
        private OperationResult(Success success)
        {
            _success = success;
        }

        private OperationResult(Failure failure)
        {
            _failure = failure;
        }
        
        public class Success : OperationResult
        {
            
        }
        
        public class Failure : OperationResult
        {
            public string Message { get; }

            public Failure(string message)
            {
                Message = message;
            }
        }
    }
}