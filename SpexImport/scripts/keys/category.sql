ALTER TABLE category_temp ADD PRIMARY KEY(categoryid);
ALTER TABLE category_temp ADD FOREIGN KEY(parentcategoryid) REFERENCES category_temp(categoryid);