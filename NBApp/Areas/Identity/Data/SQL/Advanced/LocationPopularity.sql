/*Delivery
Most popular delivery cities by order volume
Joins Orders to ShippingAddresses and groups by City — helpful for deciding where to expand or where to offer faster delivery.*/
SELECT
    sa.City,
    COUNT(o.OrderId)       AS TotalOrders,
    SUM(o.TotalAmount)    AS TotalRevenue,
    COUNT(DISTINCT o.UserId) AS UniqueCustomers
FROM Orders o
JOIN ShippingAddresses sa ON sa.ShipID = o.ShippingAddressId
WHERE o.Status NOT IN (4)  -- exclude Cancelled
GROUP BY sa.City
ORDER BY TotalOrders DESC;