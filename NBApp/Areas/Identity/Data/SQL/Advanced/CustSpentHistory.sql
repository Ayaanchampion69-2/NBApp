/*Customers
Users who have never placed an order
Uses LEFT JOIN + NULL check to find registered accounts with zero purchase history — useful for re-engagement campaigns.*/
SELECT
    u.DisplayName,
    u.Email,
    u.PhoneNumber
FROM AspNetUsers u
LEFT JOIN Orders o ON o.UserId = u.Id
WHERE o.OrderId IS NULL
ORDER BY u.DisplayName;