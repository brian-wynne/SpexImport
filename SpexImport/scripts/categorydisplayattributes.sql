CREATE TABLE IF NOT EXISTS categorydisplayattributes_temp
(
	headerid INT,
	categoryid INT,
	attributeid BIGINT,
	isactive SMALLINT,
	templatetype SMALLINT,
	displayorder SMALLINT,
	FOREIGN KEY(headerid) REFERENCES headernames(headerid),
	FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid)
);