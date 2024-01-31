// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Example.Models;
using Example.Models.Dto;
using Example.Mappers;

var registration = new UserRegistrationDto()
{
    Id = 1,
    Name = "Juan Sánchez",
    Birthday = "10/10/1999",
    Password = "U12c10y.",
    PasswordConfirmation = "U12c10y."
};

Console.WriteLine(JsonSerializer.Serialize<UserRegistrationDto>(registration));

IUserMapper userMapper = new UserMapper();

var user = userMapper.UserFromUserRegistration(registration);

Console.WriteLine(JsonSerializer.Serialize<User>(user));

var userDetail = userMapper.UserDetailFromUser(user);

Console.WriteLine(JsonSerializer.Serialize<UserDetailDto>(userDetail));

var userMinimalDetail = userMapper.UserMinimalDetailFromUser(user);

Console.WriteLine(JsonSerializer.Serialize<UserDetailDto>(userMinimalDetail));