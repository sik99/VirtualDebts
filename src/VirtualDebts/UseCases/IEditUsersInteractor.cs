using System.Threading.Tasks;

namespace VirtualDebts.UseCases
{
    public interface IEditUsersInteractor
    {
        Task AddUser(string userName);
        Task RemoveUser(string userName);
    }
}