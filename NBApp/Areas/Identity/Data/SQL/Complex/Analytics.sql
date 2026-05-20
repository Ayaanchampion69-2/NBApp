/*Analytics
Category revenue share with running total (window function)
Calculates each category's contribution to total revenue and a cumulative running total using SUM() OVER.*/
SELECT
    c.Name                                               AS Category,
    COUNT(DISTINCT o.OrderId)                           AS Orders,
    SUM(oi.Quantity * oi.UnitPrice)                    AS CategoryRevenue,
    ROUND(
        100.0 * SUM(oi.Quantity * oi.UnitPrice)
              / SUM(SUM(oi.Quantity * oi.UnitPrice)) OVER ()
    , 2)                                                 AS RevenueSharePct,
    SUM(SUM(oi.Quantity * oi.UnitPrice))
        OVER (ORDER BY SUM(oi.Quantity * oi.UnitPrice) DESC
              ROWS BETWEEN UNBOUNDED PRECEDING AND CURRENT ROW)
                                                         AS RunningTotal
FROM Categories c
JOIN Products  p  ON p.CategoryId = c.CategoryId
JOIN OrderItems oi ON oi.ProductId = p.ProductId
JOIN Orders    o  ON o.OrderId   = oi.OrderId
                  AND o.Status NOT IN (4)
GROUP BY c.CategoryId, c.Name
ORDER BY CategoryRevenue DESC;