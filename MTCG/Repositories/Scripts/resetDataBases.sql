DO
$$
BEGIN
   EXECUTE (
      SELECT string_agg('TRUNCATE TABLE ' || quote_ident(tablename) || ' CASCADE;', ' ')
      FROM pg_tables
      WHERE schemaname = 'public'
   );
END;
$$;