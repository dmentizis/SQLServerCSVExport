-- Result set 1: user tables with estimated row counts (heap/clustered-index partitions).
SELECT t.object_id, s.name, t.name,
    COALESCE((SELECT SUM(p.rows) FROM sys.partitions p
              WHERE p.object_id = t.object_id AND p.index_id IN (0, 1)), 0)
FROM sys.tables t JOIN sys.schemas s ON s.schema_id = t.schema_id
WHERE t.is_ms_shipped = 0 ORDER BY s.name, t.name;

-- Result set 2: primary-key columns in key order.
SELECT i.object_id, c.name
FROM sys.indexes i
JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
WHERE i.is_primary_key = 1 AND ic.key_ordinal > 0
ORDER BY i.object_id, ic.key_ordinal;
