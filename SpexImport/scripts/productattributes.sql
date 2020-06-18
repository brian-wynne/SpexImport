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
	type INT,
	FOREIGN KEY(productid) REFERENCES product(productid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid),
	FOREIGN KEY(unitid) REFERENCES unitnames(unitid),
	FOREIGN KEY(localeid) REFERENCES locales(localeid)
);