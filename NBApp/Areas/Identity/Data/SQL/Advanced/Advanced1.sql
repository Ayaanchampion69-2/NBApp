SELECT
	Products,
	OrderItem

FROM
	Products p
	LEFT JOIN OrderItems oi ON o.ProductId = p.ProductId

ORDER BY
	OrderItemId DESC;