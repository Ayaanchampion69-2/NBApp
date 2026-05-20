/*Top customers by lifetime value with order breakdown
Ranks users by total spend, including order count and average order size — useful for loyalty tiers or marketing segments.*/
SELECT
    u.Email,
    u.DisplayName,
    COUNT(o.OrderId)                                   AS TotalOrders,
    SUM(o.TotalAmount)                                  AS LifetimeValue,
    ROUND(AVG(o.TotalAmount), 2)                        AS AvgOrderValue,
    MAX(o.OrderDate)                                     AS MostRecentOrder,
    STRING_AGG(CAST(o.Status AS VARCHAR), ', ')           AS AllStatuses
FROM AspNetUsers u
JOIN Orders o ON o.UserId = u.Id
GROUP BY u.Id, u.Email, u.DisplayName
HAVING COUNT(o.OrderId) >= 1
ORDER BY LifetimeValue DESC;