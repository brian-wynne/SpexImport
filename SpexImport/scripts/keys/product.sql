ALTER TABLE product_temp
	ADD PRIMARY KEY(productid),
	ADD FOREIGN KEY(manufacturerid) REFERENCES manufacturer(manufacturerid),
	ADD FOREIGN KEY(categoryid) REFERENCES category(categoryid);

CREATE INDEX mfg_pn ON product_temp(mfgpn);