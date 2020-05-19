USE spex;

-- product
ALTER TABLE product ADD PRIMARY KEY (productid);
CREATE INDEX mfg_pn ON product(mfgpn);

-- locales
ALTER TABLE locales ADD PRIMARY KEY(localeid);

-- unit names
ALTER TABLE unitnames ADD PRIMARY KEY(unitid);

-- category names
ALTER TABLE categorynames ADD PRIMARY KEY(categoryid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- headernames
ALTER TABLE headernames ADD PRIMARY KEY(headerid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- product feature bullets
ALTER TABLE productfeaturebullets ADD PRIMARY KEY(uniqueid),
ADD FOREIGN KEY(productid) REFERENCES product(productid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- attributenames
ALTER TABLE attributenames ADD PRIMARY KEY(attributeid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- productattributes
ALTER TABLE productattributes ADD FOREIGN KEY(productid) REFERENCES product(productid),
ADD FOREIGN KEY(attributeid) REFERENCES attributenames(attributeid),
ADD FOREIGN KEY(unitid) REFERENCES unitnames(unitid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- productdescriptions
ALTER TABLE productdescriptions ADD FOREIGN KEY(productid) REFERENCES product(productid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- productlocales
ALTER TABLE productlocales ADD FOREIGN KEY(productid) REFERENCES product(productid);

-- productaccessories
ALTER TABLE productaccessories ADD FOREIGN KEY(productid) REFERENCES product(productid);

-- searchattributes
ALTER TABLE searchattributes ADD FOREIGN KEY(productid) REFERENCES product(productid),
ADD FOREIGN KEY(categoryid) REFERENCES categorynames(categoryid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);

-- procutkeywords
ALTER TABLE productkeywords ADD FOREIGN KEY(productid) REFERENCES product(productid),
ADD FOREIGN KEY(localeid) REFERENCES locales(localeid);
