namespace Application.Interfaces
{
    public interface ICommand { }

    public interface ICommand<out TResult> : ICommand { }
}
