private void button5_Click(object sender, EventArgs e)
        {
            //1. 创建一个json元素
            JsonElement companyID = new JsonElement("companyID", "15");
            JsonArray employeesArry = new JsonArray("employees");
            JsonDocument employeesdoc1 = new JsonDocument();
            employeesdoc1.add(new JsonElement("firstName", "Bill"));
            employeesdoc1.add(new JsonElement("lastName", "Gates"));
            JsonDocument employeesdoc2 = new JsonDocument();
            employeesdoc2.add(new JsonElement("firstName", ""));
            employeesdoc2.add(new JsonElement("lastName", "Bush"));
            employeesArry.add(employeesdoc1);
            employeesArry.add(employeesdoc2);
            JsonArray manager = new JsonArray("manager");
            JsonDocument manager1 = new JsonDocument();
            manager1.add(new JsonElement("salary", "6000"));
            manager1.add(new JsonElement("age", "23"));
            string[] carslist =
            {
                "Porsche", "BMW", "Volvo"
            };
            JsonArray cars = new JsonArray("cars", carslist);
            JsonDocument manager2 = new JsonDocument();
            manager2.add(new JsonElement("salary", "8000"));
            manager2.add(new JsonElement("age", "26"));
            manager2.add(cars);
            manager.add(manager1);
            manager.add(manager2);
            JsonObject jo = new JsonObject("cc");
            jo.add(new JsonElement("salary", "test"));
            jo.add(new JsonElement("age", "100"));

            //组装一下总体的文档
            JsonDocument jd = new JsonDocument();
            jd.add(companyID);
            jd.add(employeesArry);
            jd.add(manager);
            jd.add(jo);

            //输出文档
            textBox1.Text = jd.innerText;
        }
