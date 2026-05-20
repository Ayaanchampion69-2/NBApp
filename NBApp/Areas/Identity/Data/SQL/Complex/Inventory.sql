/*Inventory
Low-stock products with units sold and category context
Joins stock levels against actual sales velocity — flags products that are both low in stock and high in demand.*/
SELECT
    p.Name                                         AS ProductName,
    c.Name                                         AS Category,
    p.SKUNumber,
    p.StockQuantity                                 AS CurrentStock,
    p.Price,
    COALESCE(SUM(oi.Quantity), 0)                  AS TotalUnitsSold,
    COALESCE(SUM(oi.Quantity * oi.UnitPrice), 0)   AS TotalRevenue
FROM Products p
LEFT JOIN Categories c  ON c.CategoryId = p.CategoryId
LEFT JOIN OrderItems oi ON oi.ProductId = p.ProductId
LEFT JOIN Orders o      ON o.OrderId = oi.OrderId
                        AND o.Status NOT IN (4) -- exclude Cancelled
WHERE p.IsActive = 1
  AND p.StockQuantity < 10
GROUP BY
    p.ProductId, p.Name, c.Name,
    p.SKUNumber, p.StockQuantity, p.Price
ORDER BY TotalUnitsSold DESC, p.StockQuantity ASC;