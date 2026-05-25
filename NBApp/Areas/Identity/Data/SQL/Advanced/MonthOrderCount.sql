/*Orders
Monthly order count and revenue trend
Groups delivered/shipped orders by month using YEAR() and MONTH() — shows seasonal peaks and slow periods over time.*/
SELECT
    YEAR(o.OrderDate)                      AS OrderYear,
    MONTH(o.OrderDate)                     AS OrderMonth,
    COUNT(o.OrderId)                       AS TotalOrders,
    SUM(o.TotalAmount)                    AS MonthlyRevenue,
    ROUND(AVG(o.TotalAmount), 2)          AS AvgOrderValue
FROM Orders o
WHERE o.Status IN (2, 3)  -- Shipped or Delivered only
GROUP BY YEAR(o.OrderDate), MONTH(o.OrderDate)
ORDER BY OrderYear, OrderMonth;