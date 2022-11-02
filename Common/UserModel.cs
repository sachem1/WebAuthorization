namespace Common;

public class UserModel
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Mobile { get; set; }
    public string UnionId { get; set; }
    public string OpenId { get; set; }

    public List<UserRole> UserRoles { get; set; }

}

public class UserRole
{
    public string RoleId { get; set; }

    public string RoleName { get; set; }
}