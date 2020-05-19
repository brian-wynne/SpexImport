CREATE DATABASE IF NOT EXISTS spex;

DROP TABLE IF EXISTS product;
CREATE TABLE IF NOT EXISTS product 
(
	productid INT NOT NULL, 
	mfgid VARCHAR(16), 
	mfgpn VARCHAR(128) NOT NULL, 
	categoryid INT, 
	is_active CHAR(1), 
	equivalency TEXT, 
	create_date TIMESTAMP, 
	modify_date TIMESTAMP, 
	last_update TIMESTAMP, 
	PRIMARY KEY(productid, mfgpn)
);
CREATE UNIQUE INDEX product_id ON product(productid);
CREATE INDEX category_id ON product(categoryid);
CREATE INDEX mfg_pn ON product(mfgpn);

DROP TABLE IF EXISTS productattributes;
CREATE TABLE IF NOT EXISTS productattributes 
(
	productid INT, 
	attributeid BIGINT, 
	setnumber SMALLINT, 
	text TEXT, 
	absolutevalue DOUBLE, 
	unitid INT, 
	isabsolute SMALLINT, 
	isactive SMALLINT, 
	localeid INT, 
	type INT
);
CREATE INDEX product_id ON productattributes(productid);
CREATE INDEX attribute_id ON productattributes(attributeid);

DROP TABLE IF EXISTS productdescriptions;
CREATE TABLE IF NOT EXISTS productdescriptions 
(
	productid INT, 
	description TEXT, 
	isdefault CHAR(1), 
	type CHAR(1), 
	localeid CHAR(1)
);
CREATE INDEX product_id ON productdescriptions(productid);

DROP TABLE IF EXISTS productfeaturebullets;
CREATE TABLE IF NOT EXISTS productfeaturebullets 
(
	uniqueid BIGINT, 
	productid INT, 
	localeid SMALLINT, 
	orderid SMALLINT, 
	text TEXT, 
	modifieddate TIMESTAMP
);
CREATE INDEX product_id ON productfeaturebullets(productid);

DROP TABLE IF EXISTS productlocales;
CREATE TABLE IF NOT EXISTS productlocales 
(
	productid INT, 
	isactive CHAR(1), 
	published TINYTEXT, 
	PRIMARY KEY(productid)
);
CREATE INDEX product_id ON productlocales(productid);

DROP TABLE IF EXISTS productaccessories;
CREATE TABLE IF NOT EXISTS productaccessories 
(
	productid INT, 
	accessoryid INT, 
	localeid SMALLINT
);
CREATE INDEX product_id ON productaccessories(productid);

DROP TABLE IF EXISTS searchattributes;
CREATE TABLE IF NOT EXISTS searchattributes 
(
	productid INT, 
	categoryid INT, 
	unknownid INT, 
	isactive SMALLINT, 
	localeid SMALLINT
);
CREATE INDEX product_id ON searchattributes(productid);

DROP TABLE IF EXISTS productkeywords;
CREATE TABLE IF NOT EXISTS productkeywords 
(
	productid INT, 
	text TEXT, 
	localeid SMALLINT
);
CREATE INDEX product_id ON productkeywords(productid);


-- TAX.zip
DROP TABLE IF EXISTS attributenames;
CREATE TABLE IF NOT EXISTS attributenames
(
	attributeid INT,
	name TEXT,
	localeid SMALLINT
);
CREATE INDEX attribute_id ON attributenames(attributeid);

DROP TABLE IF EXISTS categorynames;
CREATE TABLE IF NOT EXISTS categorynames
(
	categoryid INT,
	name TEXT,
	localeid SMALLINT
);
CREATE INDEX category_id ON categorynames(categoryid);

DROP TABLE IF EXISTS headernames;
CREATE TABLE IF NOT EXISTS headernames
(
	headerid INT,
	name TEXT,
	localeid SMALLINT
);
CREATE INDEX header_id ON headernames(headerid);

DROP TABLE IF EXISTS locales;
CREATE TABLE IF NOT EXISTS locales 
(
	localeid SMALLINT,
	isactive TINYINT,
	languagecode VARCHAR(5),
	countrycode VARCHAR(5),
	name TEXT
);
CREATE INDEX locale_id ON locales(localeid);

DROP TABLE IF EXISTS unitnames;
CREATE TABLE IF NOT EXISTS unitnames
(
	unitid INT,
	baseunitid INT,
	multiple DOUBLE
);
CREATE INDEX unit_id ON unitnames(unitid);
