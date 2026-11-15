namespace SmartLibrary.Users;

/// <summary>Repozitorij zamjenjuje cijeli snimak podataka; implementacija mora spremiti ili baciti izuzetak.</summary>
public interface IUserRepository
{
    IReadOnlyList<User> Load();
    void Save(IReadOnlyList<User> users);
}
