ALTER TABLE searchattributes_temp
	ADD FOREIGN KEY(productid) REFERENCES product(productid),
	ADD FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid),
	ADD FOREIGN KEY(valueid) REFERENCES searchattributevalues(valueid),
	ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);