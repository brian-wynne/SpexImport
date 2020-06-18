CREATE TABLE IF NOT EXISTS categorysearchattributes_temp
(
	categoryid INT,
	attributeid BIGINT,
	isactive SMALLINT,
	FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid)
);