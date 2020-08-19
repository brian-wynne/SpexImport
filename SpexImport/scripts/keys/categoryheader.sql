ALTER TABLE categoryheader_temp
	ADD FOREIGN KEY(headerid) REFERENCES headernames(headerid),
	ADD FOREIGN KEY(categoryid) REFERENCES category(categoryid);