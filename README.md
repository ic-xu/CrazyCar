# CrazyCar
unity 制作前端游戏；Java+MySQL+Tomcat+Nginx部署服务器

## 环境版本

1. unity 2019.4.14
2. VS 2019
3. Eclipse Mars 1.4
4. JDK 1.8.0_91
5. JRE 1.8.0_91
6. MySQL 8.0.26
7. Tomcat 8.0.52
8. Nginx  1.20.1

## 服务器环境配置

### 一、配置服务器基本属性

1. 购买云服务器 [华为云](https://www.huaweicloud.com/?locale=zh-cn)
2. 设置云服务的安全组[入口](https://console.huaweicloud.com/lcs/?agencyId=0d551b5ba600f5841fd4c0182c6aa4b6&region=cn-south-1&locale=zh-cn#/lcs/manager/vmList/vmDetail/securitygroups?instanceId=410b1a5b-ee07-434a-8fea-c2c6e3b54a7d) ，华为云初始化会有三个安全组，其中有一个Web Server组是用来做Web开发的，它已经把80、443等网络端口开放，当然也可以自己去设定比如开放8080

### 二、配置Java环境

1. 下载JDK[安装](https://www.jdkdownload.com/) ，注意不是JRE，JDK中包含JRE
2. 配置Java的环境变量
3. 在CMD输入JVAV进行测试配置是否成功

### 三、配置Web服务器

1. 下载并安装[Tomcat](https://www.jdkdownload.com/)
2. 启动Tomcat
3. 浏览器输入localhost:8080，进行测试安装是否成功
4. 下载[Nginx](http://nginx.org/en/download.html)
5. 启动Nginx
6. 浏览器输入localhost:80，进行测试安装是否成功
7. 配置Nginx，将80端口映射成8080
8. 此时就可以直接通过访问IP地址，实现对8080端口的Tomcat下的Web项目

### 四、安装Eclipse Java EE

1. 官网已经停止此安装包，只能通过三方下载
2. 创建Servlet Web项目进行测试，注意此时的Eclipse可能只支持Tomcat8.0，不支持8.5，两个版本差别只在于，8.0多了一个图像界面
3. 服务器本地测试运行正常后，将项目打包成WAR包，复制到Tomcat的webapps文件夹下
4. 重启Tomcat后，Tomcat会自动解压文件夹
5. 此时应该可以从本地访问服务器，通过类似于http://IP/TestServlet/TestServlet的地址访问服务器

### 五、安装MySQL

1. 下载并安装[MySQL](https://dev.mysql.com/downloads/mysql/)
2. 根据下载的版本，下载相关[JDBC](https://mvnrepository.com/artifact/mysql/mysql-connector-java)
3. 将下载好的JAR包，一份复制到Eclipse中的Web项目下，并导入项目中；一份复制到Tomcat下的Lib文件夹
4. 根据测试项目进行连接测试，[菜鸟教程](https://www.runoob.com/java/java-mysql-connect.html)

### 六、数据传输格式制定

1. HTTP 数据传输格式指定为JSON

2. 选定JAR包为FastJson，[使用及下载地址](https://www.runoob.com/w3cnote/fastjson-intro.html)

3. 要注意JavaWeb项目的三方JAR，要在WebContent下的WEB-INF新建lib文件夹并放入，才能正常调用

4. 由于Unity系统内置的UnityWebRequest发送的数据为byte数组，所以在服务器端需要额外处理才能使用

   ```java
   protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
   		// TODO Auto-generated method stub
   		response.setContentType("text/html");
   
   		System.out.println("读取请求内容.");
   		BufferedReader br = new BufferedReader(new 		InputStreamReader(request.getInputStream(),"utf-8"));
   		String line = null;
   		StringBuilder sb = new StringBuilder();
   		while ((line = br.readLine()) != null) {
   			sb.append(line);
   		}
   		System.out.println(sb.toString());
   		JSONObject json = JSONObject.parseObject(sb.toString());
   		if(!sb.toString().isEmpty() && json.containsKey("key")){
   			System.out.println(json.getString("key"));
   		}
   
   
   		System.out.println("开始回复消息.");
   		PrintWriter out = response.getWriter();		
   		JSONArray jsonArray = new JSONArray();
           JSONObject jsonObject = new JSONObject();
           jsonObject.put("key", "value");
           jsonArray.add(jsonObject);
           // 此处为数组
   	    String jsonOutput = jsonArray.toJSONString();
   		//out.println(jsonObject.toString());
           out.println(jsonOutput);
   		out.flush();
   		out.close();
   	}
   ```


## 工具

### 一、EasyRoads3D

1. 使用的版本是V3.1.9pro版本，free版本不能用代码获取路线数据

2. 右键 ->  3D Object -> EasyRoads3D -> New Road Network

3. 在Road Network物体上选择`“＋”`进行新建路

4. 通过 `Shift` + 左键进行路线编辑

5. 新建空物体物体 `RouteNet`，并在它下面建立空子物体`Line0001`，分别赋值脚本`RouteNet.cs`以及`CatmullRomSpline.cs`，并给两个脚本赋值上相关引用和初始值

   内容为：

   ```c#
   using System.Collections.Generic;
   using UnityEngine;
   
   public class RouteNet : MonoBehaviour
   {
       [SerializeField] public CatmullRomSpline[] allRoutes;
   }
   ```

   ```c#
   using System.Collections.Generic;
   using UnityEngine;
   
   public partial class CatmullRomSpline : MonoBehaviour
   {
       public string __roadName;
       public int resolution;
       public List<Vector3> positions;
   }
   ```

6. 新建编辑器脚本，并放入`Editor`文件夹

   ```c#
   using UnityEditor;
   using UnityEngine;
   using EasyRoads3Dv3;
   using System.Collections.Generic;
   
   [CustomEditor(typeof(RouteNet))]
   public class RouteNetEditor : Editor
   {
   
       private RouteNet rNet;
   
       public ERRoad markers;
   
       public override void OnInspectorGUI() {
           rNet = target as RouteNet;
           base.OnInspectorGUI();
   
           EditorGUI.BeginChangeCheck();
   
           if (GUILayout.Button("BuildAllRoute")) {
               Undo.RecordObject(target, "BuildAllRoute");
   
               ERRoadNetwork net = new ERRoadNetwork();
               foreach (var r in rNet.allRoutes) {
                   Debug.Log("Road Name : " + r.__roadName + " Points Count " + r.positions.Count);
                   markers = net.GetRoadByName(r.__roadName);
                   if (markers == null) {
                       Debug.LogError("NO SUCH ROAD.....CANCELED......");
                       continue;
                   }
   
                   r.resolution = 50;
                   Vector3[] arr = markers.GetSplinePointsCenter();
                   r.positions = new List<Vector3>(arr);
                   for (int i = 0; i < r.positions.Count; i++) {
                       //Debug.Log(r.positions[i]);
                   }
               }
               EditorUtility.SetDirty(target);
           }
       }
   }
   ```

7. 此时在`RouteNet`物体上的脚本出现`BuildAllRoute`按钮，点击按钮就能获取相关路线的数据

### 二、JWT身份验证

1. 基本原理 [五分钟带你了解啥是JWT - 知乎 (zhihu.com)](https://zhuanlan.zhihu.com/p/86937325) 
2. `Unity`将`Token`放入`Authorization`部分，代码 ： `request.SetRequestHeader("Authorization", token);`
3. 后台通过 `request.getHeader("Authorization");`获取`Token`
4. 后台生成的`Token`中的`Id`部分为用户`Id`

### 三、DOTween

1. 动画插件

### 四、Cinemachine(官方插件)

1. 虚拟摄像机，用来追踪Player

## 功能

### 一、资源热更

#### 前端

```mermaid
graph LR
Start-->IsEditor{IsEditor}--No-->PostResource-->HaveNewResource{HaveNewResource}--Hava-->DownloadResource-->IsFinish--No-->Exit-->End;
IsEditor--Yes-->Login;
HaveNewResource--NotHava-->Login;
IsFinish--Yes-->Login-->End
```



#### 后台

1. 接口：`Host/Resource`

2. Parameter

   | Field | Type | Description |
   | :---- | :--- | :---------- |
   | -     | -    | -           |

3. Success Callback 

   ```
   {
       "code":200,
       "data":{
           "avatar":{
               "size":"0.1289",
               "crc":"1242346442",
               "hash":"9370cfe1c8e8884648f086b820bca347",
               "url":"avatar"
           }
       }
   }
   ```

4. Error Code

   | Field | Description  |
   | :---- | :----------- |
   | ! 200 | 拉取接口失败 |

5. Flow  Chart

   ```mermaid
   graph LR
   Start-->Resource-->GetDataFromDB-->200-->End;
   ```

#### 数据库

1. 表名：`ab_resource`

2. Parameter

   | Field              | Type         | Description |
   | :----------------- | :----------- | :---------- |
   | r_id (primary key) | int          | 资源ID      |
   | r_name             | varchar(100) | 资源名      |
   | r_hash             | VARCHAR(40)  | 资源Hash    |
   | r_crc              | VARCHAR(40)  | 资源CRC     |
   | r_url              | VARCHAR(40)  | 资源URL     |
   | r_size             | VARCHAR(40)  | 资源Size    |

#### 使用方法

> 例如头像需要给线上版本添加新的资源

1. 将新头像放入`Assets\AB\Avatar`文件夹
2. 点击`Window/Build/AB/Remote`编辑资源包
3. 完成后在`Console`上会显示资源的`CRC`和`Hash`,文件大小需要到文件管理器查看
4. 将数据更新到数据库ab_resource表中

### 二、登录

#### 前端 

```mermaid
graph LR
start-->LoginUI-->IsRemember{IsRemember}--Yes-->ClickLoginBtn-->PostLogin-->IsSucc{IsSucc}--Yes-->HomepageUI-->End;
IsRemember--No-->EditUserName-->ClickLoginBtn;
IsSucc--No-->EditUserName;
```



#### 后台

1. 接口：`Host/Login`

2. Parameter

   | Field    | Type   | Description |
   | :------- | :----- | :---------- |
   | UserName | string | 用户名      |
   | Password | string | 密码        |

3. Success Callback 

   ```
   {
       "code":200,
       "data":{
           "user_info":{
               "uid":1,
               "star":13,
               "name":"tast",
               "aid":12
           },
           								"token":"eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxIiwiaWF0IjoxNjMxOTQ1Njc5LCJzdWIiOiJDcmF6eUNhciIsImlzcyI6IlRhc3RTb25nIiwiZXhwIjoxNjMxOTUxNjc5fQ.7_5peioQaU1XmMR238H0sIcdlJjUG2XbzHrD-9PojUY"
       }
   }
   ```

4. Error Code

   | Field | Description |
   | :---- | :---------- |
   | 423   | 密码错误    |
   | 404   | 用户未注册  |

5. Flow Chart

   ```mermaid
   graph LR
   Start-->Login-->IsExistUser{IsExistUser}--Yes-->IsPasswordRight{IsPasswordRight}--Right-->CreateToken-->200-->End;
   IsExistUser--No-->404-->End;
   IsPasswordRight--No-->423-->End;
   ```

   

#### 数据库

1. 表名：`all_user`

2. Parameter

   | Field             | Type         | Description |
   | :---------------- | :----------- | :---------- |
   | uid (primary key) | int          | 用户ID      |
   | user_name         | varchar(100) | 用户名      |
   | user_password     | VARCHAR(40)  | 密码        |
   | login_time        | int          | 注册时间    |
   | aid               | int          | 当前头像ID  |
   | star              | int          | 星星数      |

### 三、注册

#### 前端 

```mermaid
graph LR
start-->LoginUI-->ClickRegisterBotton-->EidtUserName-->ClickRegisterBtn-->IsLegalInput{IsLegalInput}--Yes-->PostRequest-->IsSucc{IsSucc}--Yes-->HomepageUI-->End;
IsLegalInput--No-->EidtUserName;
IsSucc--No-->EidtUserName;
```



#### 后台

1. 接口：`Host/Register`

2. Parameter

   | Field    | Type   | Description |
   | :------- | :----- | :---------- |
   | UserName | string | 用户名      |
   | Password | string | 密码        |

3. Success Callback 

   ```
   {
       "code":200,
       "data":{
           "user_info":{
               "uid":7,
               "star":14,
               "name":"Taylor",
               "aid":1
           },
           "token":"eyJhbGciOiJIUzI1NiJ9.eyJqdGkiOiI3IiwiaWF0IjoxNjMxOTQ5NTQ1LCJzdWIiOiJDcmF6eUNhciIsImlzcyI6IlRhc3RTb25nIiwiZXhwIjoxNjMxOTU1NTQ1fQ.d82x7zabQtVaXogi5wdp0M1tPz4ybyi_3grsThnL83c"
       }
   }
   ```

4. Error Code

   | Field | Description  |
   | :---- | :----------- |
   | 423   | 用户已注册   |
   | 425   | 信息格式不对 |

5. Flow Chat

   ```mermaid
   graph LR
   Start-->Register-->IsExistUser{IsExistUser}--Yes-->423-->End;
   IsExistUser--No-->RegisterUser-->IsExistUser2{IsExistUser}--Yes-->CreateToken-->200-->End;
   IsExistUser2--No-->425-->End;
   ```

   

#### 数据库

1. 表名：`all_user`

2. Parameter

   | Field             | Type         | Description |
   | :---------------- | :----------- | :---------- |
   | uid (primary key) | int          | 用户ID      |
   | user_name         | varchar(100) | 用户名      |
   | user_password     | VARCHAR(40)  | 密码        |
   | login_time        | int          | 注册时间    |
   | aid               | int          | 当前头像ID  |
   | star              | int          | 星星数      |

### 四、头像

#### 前端 

```mermaid
graph TB
start-->ClickHomepageAvatarBtn-->PostAvatar-->UpdataUI;
UpdataUI-->ClickUnlockAvatar-->ClickApplyBtn-->PostChangeAvatar-->IsSucc0{IsSucc}--Yes-->UpdataCurAvatarUI-->ChangeCurAvatar-->End;
UpdataUI-->ClickLockAvatar-->IsBuy{IsBuy}--Yes-->IsEnoughStar--Yes-->PostBuyAvatar-->IsSucc1{IsSucc}--Yes-->ClickUnlockAvatar;
IsBuy--No-->End;
IsEnoughStar--No-->End;
IsSucc1--No-->End;
```



#### 后台

1. * 接口：`Host/Avarar`

   * Parameter

     | Field | Type | Description |
     | :---- | :--- | :---------- |
     | -     | -    | -           |

   * Success Callback 

     ```
     {
        "code":200,
         "data":{
             "current_aid":12,
             "avatars":[
                 {
                     "star":3,
                     "name":"Tast 0",
                     "is_has":true,
                     "aid":0
                 },
                 {
                     "star":2,
                     "name":"Black 1",
                     "is_has":false,
                     "aid":1
                 }
             ]
         }
     }
     ```

   * Error Code

     | Field | Description |
     | :---- | :---------- |
     | -     | -           |

   * Flow Chat

     ```mermaid
     graph LR
     Start-->GetUidByToken{GetUidByToken}--Yes-->GetAllAvatarID-->isHasAvatar-->200-->End;
     GetUidByToken--No-->End;
     ```

     

2. * 接口：`Host/ChangeAvatar`

   * Parameter

     | Field | Type | Description      |
     | :---- | :--- | :--------------- |
     | aid   | int  | 想要切换的头像ID |

   * Success Callback 

     ```
     {
         "code":200,
         "data":{
             "aid":2
         }
     }
     ```

   * Error Code

     | Field | Description |
     | :---- | :---------- |
     | 423   | 未拥有      |

   * Flow Chat

     ```mermaid
     graph LR
     Start-->GetUidByToken{GetUidByToken}--Yes-->GetAidByRequest{GetAidByRequest}--Yes-->IsHavaAvatar{IsHavaAvatar}--Yes-->SetAvatar-->200-->End;
     GetUidByToken--No-->End;
     GetAidByRequest--No-->404-->End;
     IsHavaAvatar--No-->423-->End;
     ```

     

3. * 接口：`Host/BuyAvatar`

   * Parameter

     | Field | Type | Description      |
     | :---- | :--- | :--------------- |
     | aid   | int  | 想要购买的头像ID |

   * Success Callback 

     ```
     {
         "code":200,
         "data":{
             "star":10
         }
     }
     ```

   * Error Code

     | Field | Description |
     | :---- | :---------- |
     | 423   | 星星不足    |
     
   * Flow Chat
   
     ```mermaid
     graph LR
     Start-->GetUidByToken{GetUidByToken}--Yes-->GetAidByRequest{GetAidByRequest}--Yes-->IsHavaAvatar{IsHavaAvatar}--Yes-->200-->End;
     GetUidByToken--No-->End;
     GetAidByRequest--No-->404-->End;
     IsHavaAvatar--No-->CanBuyAvatar{CanBuyAvatar}--Yes-->BuyAvatar-->200;
     CanBuyAvatar--No-->423-->End;
     ```
   
     

#### 数据库

1. * 表名：`avatar_name`

   * Parameter

     | Field            | Type        | Description    |
     | :--------------- | :---------- | :------------- |
     | id (primary key) | int         | ID             |
     | aid              | int         | 头像ID         |
     | avatar_name      | VARCHAR(40) | 头像名         |
     | star             | int         | 开锁所需星星数 |

2. * 表名：`avatar_uid`

   * Parameter

     | Field            | Type | Description |
     | :--------------- | :--- | :---------- |
     | id (primary key) | int  | ID          |
     | aid              | int  | 头像ID      |
     | uid              | int  | 用户ID      |

### 五、计时赛详情

#### 前端 

```mermaid
graph TB
start-->PostTimeTrialDetail-->UpdataUI-->ClickUnlock-->JoinGame-->End;
UpdataUI-->ClickLock-->IsBuy{IsBuy}--Yes-->IsEnoughStar--Yes-->PostBuy-->IsSucc{IsSucc}--Yes-->ClickUnlock;
IsBuy--No-->End;
IsEnoughStar--No-->End;
IsSucc--No-->End;
```



#### 后台

1. * 接口：`Host/TimeTrialDetail`

   * Parameter

     | Field | Type | Description |
     | :---- | :--- | :---------- |
     | -     | -    | -           |

   * Success Callback 

     ```
     {
         "code":200,
         "data":[
             {
                 "star":2,
                 "map_id":0,
                 "name":"Map 0",
                 "is_has":true,
                 "cid":0,
                 "limit_time":60
             },
             {
                 "star":1,
                 "map_id":1,
                 "name":"Map 1",
                 "is_has":true,
                 "cid":1,
                 "limit_time":70
             }
         ]
     }
     ```

   * Error Code

     | Field | Description |
     | :---- | :---------- |
     | -     | -           |

   * Flow Chat

     ```mermaid
     graph LR
     Start-->GetUidByToken{GetUidByToken}--Yes-->GetAllClassID-->200-->End;
     GetUidByToken--No-->End;
     ```

     

2. * 接口：`Host/BuyTimeTrialClass

   * Parameter

     | Field | Type | Description      |
     | :---- | :--- | :--------------- |
     | cid   | int  | 想要购买的课程ID |

   * Success Callback 

     ```
     {
         "code":200,
         "data":{
             "star":10
         }
     }
     ```

   * Error Code

     | Field | Description |
     | :---- | :---------- |
     | 423   | 星星不足    |
     
   * Flow Chat
   
     ```mermaid
     graph LR
     Start-->GetUidByToken{GetUidByToken}--Yes-->GetCidByRequest{GetCidByRequest}--Yes-->IsHavaClass{IsHavaClass}--Yes-->200-->End;
     GetUidByToken--No-->End;
     GetCidByRequest--No-->404-->End;
     IsHavaClass--No-->CanBuyClass{CanBuyClass}--Yes-->BuyClass-->200;
     CanBuyClass--No-->423-->End;
     ```
   
     

#### 数据库

1. * 表名：`time_trial_class`

   * Parameter

     | Field             | Type        | Description    |
     | :---------------- | :---------- | :------------- |
     | cid (primary key) | int         | 课程ID         |
     | map_id            | int         | 地图ID         |
     | class_name        | VARCHAR(40) | 课程名         |
     | star              | int         | 开锁所需星星数 |
     | limit_time        | int         | 限制时间       |

2. * 表名：`time_trial_user_map`

   * Parameter

     | Field            | Type | Description |
     | :--------------- | :--- | :---------- |
     | id (primary key) | int  | ID          |
     | cid              | int  | 课程ID      |
     | uid              | int  | 用户ID      |

### 六、计时赛记录

#### 前端 

```mermaid
graph LR
start-->JoinGame-->IsArriveLimitTime-->TimeTrialResultUI-->PostResult-->IsSucc{IsSucc}--Yes-->ShowRankAndResult-->End;
JoinGame-->CompleteGame-->TimeTrialResultUI;
IsSucc--No-->End;
```



#### 后台

1. 接口：`Host/TimeTrialResult`

2. Parameter

   | Field         | Type | Description            |
   | :------------ | :--- | :--------------------- |
   | uid           | int  | 用户ID                 |
   | cid           | int  | 课程ID                 |
   | complete_time | int  | 完成时间(-1代表未完成) |

3. Success Callback 

   ```
   {
       "code":200,
       "data":{
           "is_break_record":false,
           "complete_time":16,
           "is_win":true, // 按时完成比赛
           "rank":-1 // 为打破之前成绩记录，所以成绩不上榜
       }
   }
   ```

4. Error Code

   | Field | Description |
   | :---- | :---------- |
   | -     | -           |
   
5. Flow Chat

   ```mermaid
   graph LR
   Start-->IsLegalJWT{IsLegalJWT}--Yes-->GetDataByRequest{GetDataByRequest}--Yes-->InsertDataToDB-->200-->End;
   IsLegalJWT-->End;
   GetDataByRequest--No-->404-->End;
   ```

   

#### 数据库

1. 表名：`time_trial_record`

2. Parameter

   | Field            | Type | Description  |
   | :--------------- | :--- | :----------- |
   | id (primary key) | int  | ID           |
   | uid              | int  | 用户ID       |
   | cid              | int  | 课程ID       |
   | complete_time    | int  | 完成时间     |
   | record_time      | int  | 上传记录时间 |

### 七、计时赛榜单

#### 前端 

```mermaid
graph LR
start-->TimeTrialResultUI-->PostRankRequest-->IsSucc{IsSucc}--Yes-->ShowRank-->End;
IsSucc--No-->End;
```



#### 后台

1. 接口：`Host/TimeTrialRank`

2. Parameter

   | Field | Type | Description |
   | :---- | :--- | :---------- |
   | cid   | int  | 课程ID      |

3. Success Callback 

   ```
   {
       "code":200,
       "data":[
           {
               "complete_time":10,
               "name":"qwe",
               "rank":1,
               "aid":2
           },
           {
               "complete_time":14,
               "name":"Tast",
               "rank":2,
               "aid":2
           },
           {
               "complete_time":14,
               "name":"asd",
               "rank":3,
               "aid":1
           }
       ]
   }
   ```

4. Error Code

   | Field | Description |
   | :---- | :---------- |
   | -     | -           |
   
5. Flow Chat

   ```mermaid
   graph LR
   Start-->IsLegalJWT{IsLegalJWT}--Yes-->GetDataByRequest{GetDataByRequest}--Yes-->GetDataByDB-->200-->End;
   IsLegalJWT-->End;
   GetDataByRequest--No-->404-->End;
   ```

   

#### 数据库

1. 表名：`time_trial_rank_0` 临时表

2. Parameter

   | Field             | Type | Description |
   | :---------------- | :--- | :---------- |
   | uid (primary key) | int  | 用户ID      |
   | rank              | int  | 排名        |
   | aid               | int  | 用户头像ID  |
   | complete_time     | int  | 完成时间    |
   | name              | int  | 用户名      |







