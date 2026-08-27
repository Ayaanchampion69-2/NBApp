-- 1. Confirm your account's role(s)
SELECT u.UserName, r.Name AS RoleName
FROM AspNetUsers u
JOIN AspNetUserRoles ur ON u.Id = ur.UserId
JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE u.UserName = 'your_admin_email_here';

-- 2. Check what permissions exist for Admin
SELECT *
FROM FeaturePermissions
WHERE RoleName = 'Admin';