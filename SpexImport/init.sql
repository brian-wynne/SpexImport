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
	PRIMARY KEY(productid, mfgpn),
	FOREIGN KEY(categoryid, mfgid)
);

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
	type INT,
	FOREIGN KEY(productid, attributeid, categoryid, unitid, localeid)
);

DROP TABLE IF EXISTS productdescriptions;
CREATE TABLE IF NOT EXISTS productdescriptions 
(
	productid INT, 
	description TEXT, 
	isdefault CHAR(1), 
	type CHAR(1), 
	localeid CHAR(1),
	FOREIGN KEY(productid, localeid)
);

DROP TABLE IF EXISTS productfeaturebullets;
CREATE TABLE IF NOT EXISTS productfeaturebullets 
(
	uniqueid BIGINT, 
	productid INT, 
	localeid SMALLINT, 
	orderid SMALLINT, 
	text TEXT, 
	modifieddate TIMESTAMP,
	PRIMARY KEY(uniqueid),
	FOREIGN KEY(productid, localeid)
);

DROP TABLE IF EXISTS productlocales;
CREATE TABLE IF NOT EXISTS productlocales 
(
	productid INT, 
	isactive CHAR(1), 
	published TINYTEXT, 
	FOREIGN KEY(productid, localeid)
);

DROP TABLE IF EXISTS productaccessories;
CREATE TABLE IF NOT EXISTS productaccessories 
(
	productid INT, 
	accessoryid INT, 
	localeid SMALLINT,
	FOREIGN KEY(productid, localeid)
);

DROP TABLE IF EXISTS searchattributes;
CREATE TABLE IF NOT EXISTS searchattributes 
(
	productid INT, 
	categoryid INT, 
	unknownid INT, 
	isactive SMALLINT, 
	localeid SMALLINT,
	FOREIGN KEY(productid, categoryid, localeid)
);

DROP TABLE IF EXISTS productkeywords;
CREATE TABLE IF NOT EXISTS productkeywords 
(
	productid INT, 
	text TEXT, 
	localeid SMALLINT,
	FOREIGN KEY(productid, localeid)
);


-- TAX.zip
DROP TABLE IF EXISTS attributenames;
CREATE TABLE IF NOT EXISTS attributenames
(
	attributeid INT,
	name TEXT,
	localeid SMALLINT,
	PRIMARY KEY(attributeid),
	FOREIGN KEY(localeid)
);

DROP TABLE IF EXISTS categorynames;
CREATE TABLE IF NOT EXISTS categorynames
(
	categoryid INT,
	name TEXT,
	localeid SMALLINT,
	FOREIGN KEY(productid, localeid)
);

DROP TABLE IF EXISTS headernames;
CREATE TABLE IF NOT EXISTS headernames
(
	headerid INT,
	name TEXT,
	localeid SMALLINT
);

DROP TABLE IF EXISTS locales;
CREATE TABLE IF NOT EXISTS locales 
(
	localeid SMALLINT,
	isactive TINYINT,
	languagecode VARCHAR(5),
	countrycode VARCHAR(5),
	name TEXT,
	PRIMARY KEY(localeid)
);

DROP TABLE IF EXISTS unitnames;
CREATE TABLE IF NOT EXISTS unitnames
(
	unitid INT,
	baseunitid INT,
	multiple DOUBLE,
	PRIMARY KEY(unitid)
);
