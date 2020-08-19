CREATE TABLE IF NOT EXISTS product_temp
(
	productid INT NOT NULL, 
	manufacturerid INT,
	mfgpn VARCHAR(128) NOT NULL, 
	categoryid INT, 
	isaccessory TINYINT,
	equivalency TEXT, 
	create_date TIMESTAMP, 
	modify_date TIMESTAMP, 
	last_update TIMESTAMP
);