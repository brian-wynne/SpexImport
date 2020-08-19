CREATE TABLE IF NOT EXISTS productfeaturebullets_temp
(
	uniqueid BIGINT, 
	productid INT, 
	localeid SMALLINT, 
	orderid SMALLINT, 
	text TEXT, 
	modifieddate TIMESTAMP
);