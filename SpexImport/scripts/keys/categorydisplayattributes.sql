ALTER TABLE categorydisplayattributes_temp
	ADD FOREIGN KEY(headerid) REFERENCES headernames(headerid),
	ADD FOREIGN KEY(categoryid) REFERENCES category(categoryid),
	ADD FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid);