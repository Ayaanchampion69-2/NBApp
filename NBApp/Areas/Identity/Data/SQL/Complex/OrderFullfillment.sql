/*Fulfillment
Orders stuck in Pending/Processing with full shipping detail
Uses a CTE to isolate unresolved orders older than N days, then joins address and user info for a fulfillment work queue.*/
WITH StuckOrders AS (
    SELECT o.*,
           DATEDIFF(DAY, o.OrderDate, GETDATE()) AS DaysWaiting
    FROM Orders o
    WHERE o.Status IN (0, 1) -- Pending=0, Processing=1
      AND DATEDIFF(DAY, o.OrderDate, GETDATE()) > 3
)
SELECT
    so.OrderId,
    so.OrderDate,
    so.DaysWaiting,
    so.TotalAmount,
    so.Status,
    u.Email          AS CustomerEmail,
    u.DisplayName,
    sa.BuildingNumber,
    sa.Street,
    sa.City,
    sa.PostalCode,
    COUNT(oi.OrderItemId) AS LineItems
FROM StuckOrders so
JOIN  AspNetUsers    u  ON u.Id              = so.UserId
LEFT JOIN ShippingAddresses sa ON sa.ShipID     = so.ShippingAddressId
LEFT JOIN OrderItem        oi ON oi.OrderId   = so.OrderId
GROUP BY
    so.OrderId, so.OrderDate, so.DaysWaiting, so.TotalAmount, so.Status,
    u.Email, u.DisplayName,
    sa.BuildingNumber, sa.Street, sa.City, sa.PostalCode
ORDER BY so.DaysWaiting DESC;