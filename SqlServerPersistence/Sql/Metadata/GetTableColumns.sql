-- Use the underlying system type for SQL alias types, and the named type for CLR types.
SELECT c.name, COALESCE(bt.name, ut.name)
FROM sys.columns c
JOIN sys.types ut ON ut.user_type_id = c.user_type_id
LEFT JOIN sys.types bt ON bt.user_type_id = c.system_type_id AND bt.system_type_id = c.system_type_id
WHERE c.object_id = @ObjectId AND c.is_hidden = 0
ORDER BY c.column_id;
