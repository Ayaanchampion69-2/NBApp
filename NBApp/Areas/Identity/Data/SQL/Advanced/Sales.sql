/*Sales
Total revenue and units sold per product
Basic aggregation across OrderItems — great for a simple sales dashboard or product performance report.*/
SELECT
    p.Name                             AS ProductName,
    p.Price                            AS ListPrice,
    COUNT(DISTINCT oi.OrderId)        AS OrdersContaining,
    SUM(oi.Quantity)                  AS UnitsSold,
    SUM(oi.Quantity * oi.UnitPrice)  AS TotalRevenue,
    ROUND(AVG(oi.UnitPrice), 2)       AS AvgSalePrice
FROM Products p
JOIN OrderItem oi ON oi.ProductId = p.ProductId
JOIN Orders     o  ON o.OrderId   = oi.OrderId
                   AND o.Status NOT IN (4)  -- exclude Cancelled
GROUP BY p.ProductId, p.Name, p.Price
ORDER BY TotalRevenue DESC;