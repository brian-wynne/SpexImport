CREATE TABLE IF NOT EXISTS productattributes_temp 
(
	productid INT, 
	attributeid BIGINT, 
	setnumber SMALLINT, 
	text TEXT, 
	absolutevalue DOUBLE, 
	unitid INT, 
	isabsolute SMALLINT, 
	isactive SMALLINT, 
	localeid SMALLINT, 
	type INT
);