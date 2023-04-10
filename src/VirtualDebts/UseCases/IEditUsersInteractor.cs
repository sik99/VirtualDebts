using System.Threading.Tasks;
using VirtualDebts.Models;

namespace VirtualDebts.UseCases;

public interface IEditUsersInteractor
{
    Task AddUser(string userName);
    Task RemoveUser(UserIdentity userIdentity);
}