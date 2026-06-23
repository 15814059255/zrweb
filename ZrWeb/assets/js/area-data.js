// 省市区数据
var AreaData = {
    provinces: [
        { id: "110000", name: "北京市" },
        { id: "120000", name: "天津市" },
        { id: "130000", name: "河北省" },
        { id: "140000", name: "山西省" },
        { id: "150000", name: "内蒙古自治区" },
        { id: "210000", name: "辽宁省" },
        { id: "220000", name: "吉林省" },
        { id: "230000", name: "黑龙江省" },
        { id: "310000", name: "上海市" },
        { id: "320000", name: "江苏省" },
        { id: "330000", name: "浙江省" },
        { id: "340000", name: "安徽省" },
        { id: "350000", name: "福建省" },
        { id: "360000", name: "江西省" },
        { id: "370000", name: "山东省" },
        { id: "410000", name: "河南省" },
        { id: "420000", name: "湖北省" },
        { id: "430000", name: "湖南省" },
        { id: "440000", name: "广东省" },
        { id: "450000", name: "广西壮族自治区" },
        { id: "460000", name: "海南省" },
        { id: "500000", name: "重庆市" },
        { id: "510000", name: "四川省" },
        { id: "520000", name: "贵州省" },
        { id: "530000", name: "云南省" },
        { id: "540000", name: "西藏自治区" },
        { id: "610000", name: "陕西省" },
        { id: "620000", name: "甘肃省" },
        { id: "630000", name: "青海省" },
        { id: "640000", name: "宁夏回族自治区" },
        { id: "650000", name: "新疆维吾尔自治区" },
        { id: "710000", name: "台湾省" },
        { id: "810000", name: "香港特别行政区" },
        { id: "820000", name: "澳门特别行政区" }
    ],
    cities: {},
    districts: {}
};

// 初始化城市数据
AreaData.cities["110000"] = [{ id: "110100", name: "市辖区" }];
AreaData.cities["120000"] = [{ id: "120100", name: "市辖区" }];
AreaData.cities["310000"] = [{ id: "310100", name: "市辖区" }];
AreaData.cities["500000"] = [{ id: "500100", name: "市辖区" }];

AreaData.cities["130000"] = [
    { id: "130100", name: "石家庄市" }, { id: "130200", name: "唐山市" },
    { id: "130300", name: "秦皇岛市" }, { id: "130400", name: "邯郸市" },
    { id: "130500", name: "邢台市" }, { id: "130600", name: "保定市" },
    { id: "130700", name: "张家口市" }, { id: "130800", name: "承德市" },
    { id: "130900", name: "沧州市" }, { id: "131000", name: "廊坊市" },
    { id: "131100", name: "衡水市" }
];

AreaData.cities["140000"] = [
    { id: "140100", name: "太原市" }, { id: "140200", name: "大同市" },
    { id: "140300", name: "阳泉市" }, { id: "140400", name: "长治市" },
    { id: "140500", name: "晋城市" }, { id: "140600", name: "朔州市" },
    { id: "140700", name: "晋中市" }, { id: "140800", name: "运城市" },
    { id: "140900", name: "忻州市" }, { id: "141000", name: "临汾市" },
    { id: "141100", name: "吕梁市" }
];

AreaData.cities["150000"] = [
    { id: "150100", name: "呼和浩特市" }, { id: "150200", name: "包头市" },
    { id: "150300", name: "乌海市" }, { id: "150400", name: "赤峰市" },
    { id: "150500", name: "通辽市" }, { id: "150600", name: "鄂尔多斯市" },
    { id: "150700", name: "呼伦贝尔市" }, { id: "150800", name: "巴彦淖尔市" },
    { id: "150900", name: "乌兰察布市" }, { id: "152200", name: "兴安盟" },
    { id: "152500", name: "锡林郭勒盟" }, { id: "152900", name: "阿拉善盟" }
];

AreaData.cities["210000"] = [
    { id: "210100", name: "沈阳市" }, { id: "210200", name: "大连市" },
    { id: "210300", name: "鞍山市" }, { id: "210400", name: "抚顺市" },
    { id: "210500", name: "本溪市" }, { id: "210600", name: "丹东市" },
    { id: "210700", name: "锦州市" }, { id: "210800", name: "营口市" },
    { id: "210900", name: "阜新市" }, { id: "211000", name: "辽阳市" },
    { id: "211100", name: "盘锦市" }, { id: "211200", name: "铁岭市" },
    { id: "211300", name: "朝阳市" }, { id: "211400", name: "葫芦岛市" }
];

AreaData.cities["220000"] = [
    { id: "220100", name: "长春市" }, { id: "220200", name: "吉林市" },
    { id: "220300", name: "四平市" }, { id: "220400", name: "辽源市" },
    { id: "220500", name: "通化市" }, { id: "220600", name: "白山市" },
    { id: "220700", name: "松原市" }, { id: "220800", name: "白城市" },
    { id: "222400", name: "延边朝鲜族自治州" }
];

AreaData.cities["230000"] = [
    { id: "230100", name: "哈尔滨市" }, { id: "230200", name: "齐齐哈尔市" },
    { id: "230300", name: "鸡西市" }, { id: "230400", name: "鹤岗市" },
    { id: "230500", name: "双鸭山市" }, { id: "230600", name: "大庆市" },
    { id: "230700", name: "伊春市" }, { id: "230800", name: "佳木斯市" },
    { id: "230900", name: "七台河市" }, { id: "231000", name: "牡丹江市" },
    { id: "231100", name: "黑河市" }, { id: "231200", name: "绥化市" },
    { id: "232700", name: "大兴安岭地区" }
];

AreaData.cities["320000"] = [
    { id: "320100", name: "南京市" }, { id: "320200", name: "无锡市" },
    { id: "320300", name: "徐州市" }, { id: "320400", name: "常州市" },
    { id: "320500", name: "苏州市" }, { id: "320600", name: "南通市" },
    { id: "320700", name: "连云港市" }, { id: "320800", name: "淮安市" },
    { id: "320900", name: "盐城市" }, { id: "321000", name: "扬州市" },
    { id: "321100", name: "镇江市" }, { id: "321200", name: "泰州市" },
    { id: "321300", name: "宿迁市" }
];

AreaData.cities["330000"] = [
    { id: "330100", name: "杭州市" }, { id: "330200", name: "宁波市" },
    { id: "330300", name: "温州市" }, { id: "330400", name: "嘉兴市" },
    { id: "330500", name: "湖州市" }, { id: "330600", name: "绍兴市" },
    { id: "330700", name: "金华市" }, { id: "330800", name: "衢州市" },
    { id: "330900", name: "舟山市" }, { id: "331000", name: "台州市" },
    { id: "331100", name: "丽水市" }
];

AreaData.cities["340000"] = [
    { id: "340100", name: "合肥市" }, { id: "340200", name: "芜湖市" },
    { id: "340300", name: "蚌埠市" }, { id: "340400", name: "淮南市" },
    { id: "340500", name: "马鞍山市" }, { id: "340600", name: "淮北市" },
    { id: "340700", name: "铜陵市" }, { id: "340800", name: "安庆市" },
    { id: "341000", name: "黄山市" }, { id: "341100", name: "滁州市" },
    { id: "341200", name: "阜阳市" }, { id: "341300", name: "宿州市" },
    { id: "341500", name: "六安市" }, { id: "341600", name: "亳州市" },
    { id: "341700", name: "池州市" }, { id: "341800", name: "宣城市" }
];

AreaData.cities["350000"] = [
    { id: "350100", name: "福州市" }, { id: "350200", name: "厦门市" },
    { id: "350300", name: "莆田市" }, { id: "350400", name: "三明市" },
    { id: "350500", name: "泉州市" }, { id: "350600", name: "漳州市" },
    { id: "350700", name: "南平市" }, { id: "350800", name: "龙岩市" },
    { id: "350900", name: "宁德市" }
];

AreaData.cities["360000"] = [
    { id: "360100", name: "南昌市" }, { id: "360200", name: "景德镇市" },
    { id: "360300", name: "萍乡市" }, { id: "360400", name: "九江市" },
    { id: "360500", name: "新余市" }, { id: "360600", name: "鹰潭市" },
    { id: "360700", name: "赣州市" }, { id: "360800", name: "吉安市" },
    { id: "360900", name: "宜春市" }, { id: "361000", name: "抚州市" },
    { id: "361100", name: "上饶市" }
];

AreaData.cities["370000"] = [
    { id: "370100", name: "济南市" }, { id: "370200", name: "青岛市" },
    { id: "370300", name: "淄博市" }, { id: "370400", name: "枣庄市" },
    { id: "370500", name: "东营市" }, { id: "370600", name: "烟台市" },
    { id: "370700", name: "潍坊市" }, { id: "370800", name: "济宁市" },
    { id: "370900", name: "泰安市" }, { id: "371000", name: "威海市" },
    { id: "371100", name: "日照市" }, { id: "371200", name: "临沂市" },
    { id: "371300", name: "德州市" }, { id: "371400", name: "聊城市" },
    { id: "371500", name: "滨州市" }, { id: "371600", name: "菏泽市" }
];

AreaData.cities["410000"] = [
    { id: "410100", name: "郑州市" }, { id: "410200", name: "开封市" },
    { id: "410300", name: "洛阳市" }, { id: "410400", name: "平顶山市" },
    { id: "410500", name: "安阳市" }, { id: "410600", name: "鹤壁市" },
    { id: "410700", name: "新乡市" }, { id: "410800", name: "焦作市" },
    { id: "410900", name: "濮阳市" }, { id: "411000", name: "许昌市" },
    { id: "411100", name: "漯河市" }, { id: "411200", name: "三门峡市" },
    { id: "411300", name: "南阳市" }, { id: "411400", name: "商丘市" },
    { id: "411500", name: "信阳市" }, { id: "411600", name: "周口市" },
    { id: "411700", name: "驻马店市" }, { id: "419001", name: "济源市" }
];

AreaData.cities["420000"] = [
    { id: "420100", name: "武汉市" }, { id: "420200", name: "黄石市" },
    { id: "420300", name: "十堰市" }, { id: "420500", name: "宜昌市" },
    { id: "420600", name: "襄阳市" }, { id: "420700", name: "鄂州市" },
    { id: "420800", name: "荆门市" }, { id: "420900", name: "孝感市" },
    { id: "421000", name: "荆州市" }, { id: "421100", name: "黄冈市" },
    { id: "421200", name: "咸宁市" }, { id: "421300", name: "随州市" },
    { id: "422800", name: "恩施土家族苗族自治州" }, { id: "429004", name: "仙桃市" },
    { id: "429005", name: "潜江市" }, { id: "429006", name: "天门市" },
    { id: "429021", name: "神农架林区" }
];

AreaData.cities["430000"] = [
    { id: "430100", name: "长沙市" }, { id: "430200", name: "株洲市" },
    { id: "430300", name: "湘潭市" }, { id: "430400", name: "衡阳市" },
    { id: "430500", name: "邵阳市" }, { id: "430600", name: "岳阳市" },
    { id: "430700", name: "常德市" }, { id: "430800", name: "张家界市" },
    { id: "430900", name: "益阳市" }, { id: "431000", name: "郴州市" },
    { id: "431100", name: "永州市" }, { id: "431200", name: "怀化市" },
    { id: "431300", name: "娄底市" }, { id: "433100", name: "湘西土家族苗族自治州" }
];

AreaData.cities["440000"] = [
    { id: "440100", name: "广州市" }, { id: "440200", name: "韶关市" },
    { id: "440300", name: "深圳市" }, { id: "440400", name: "珠海市" },
    { id: "440500", name: "汕头市" }, { id: "440600", name: "佛山市" },
    { id: "440700", name: "江门市" }, { id: "440800", name: "湛江市" },
    { id: "440900", name: "茂名市" }, { id: "441200", name: "肇庆市" },
    { id: "441300", name: "惠州市" }, { id: "441400", name: "梅州市" },
    { id: "441500", name: "汕尾市" }, { id: "441600", name: "河源市" },
    { id: "441700", name: "阳江市" }, { id: "441800", name: "清远市" },
    { id: "441900", name: "东莞市" }, { id: "442000", name: "中山市" },
    { id: "445100", name: "潮州市" }, { id: "445200", name: "揭阳市" },
    { id: "445300", name: "云浮市" }
];

AreaData.cities["450000"] = [
    { id: "450100", name: "南宁市" }, { id: "450200", name: "柳州市" },
    { id: "450300", name: "桂林市" }, { id: "450400", name: "梧州市" },
    { id: "450500", name: "北海市" }, { id: "450600", name: "防城港市" },
    { id: "450700", name: "钦州市" }, { id: "450800", name: "贵港市" },
    { id: "450900", name: "玉林市" }, { id: "451000", name: "百色市" },
    { id: "451100", name: "贺州市" }, { id: "451200", name: "河池市" },
    { id: "451300", name: "来宾市" }, { id: "451400", name: "崇左市" }
];

AreaData.cities["460000"] = [
    { id: "460100", name: "海口市" }, { id: "460200", name: "三亚市" },
    { id: "460300", name: "三沙市" }, { id: "460400", name: "儋州市" },
    { id: "469001", name: "五指山市" }, { id: "469002", name: "琼海市" },
    { id: "469005", name: "文昌市" }, { id: "469006", name: "万宁市" },
    { id: "469007", name: "东方市" }, { id: "469021", name: "定安县" },
    { id: "469022", name: "屯昌县" }, { id: "469023", name: "澄迈县" },
    { id: "469024", name: "临高县" }, { id: "469025", name: "白沙黎族自治县" },
    { id: "469026", name: "昌江黎族自治县" }, { id: "469027", name: "乐东黎族自治县" },
    { id: "469028", name: "陵水黎族自治县" }, { id: "469029", name: "保亭黎族苗族自治县" },
    { id: "469030", name: "琼中黎族苗族自治县" }
];

AreaData.cities["510000"] = [
    { id: "510100", name: "成都市" }, { id: "510300", name: "自贡市" },
    { id: "510400", name: "攀枝花市" }, { id: "510500", name: "泸州市" },
    { id: "510600", name: "德阳市" }, { id: "510700", name: "绵阳市" },
    { id: "510800", name: "广元市" }, { id: "510900", name: "遂宁市" },
    { id: "511000", name: "内江市" }, { id: "511100", name: "乐山市" },
    { id: "511300", name: "南充市" }, { id: "511400", name: "眉山市" },
    { id: "511500", name: "宜宾市" }, { id: "511600", name: "广安市" },
    { id: "511700", name: "达州市" }, { id: "511800", name: "雅安市" },
    { id: "511900", name: "巴中市" }, { id: "512000", name: "资阳市" },
    { id: "513200", name: "阿坝藏族羌族自治州" }, { id: "513300", name: "甘孜藏族自治州" },
    { id: "513400", name: "凉山彝族自治州" }
];

AreaData.cities["520000"] = [
    { id: "520100", name: "贵阳市" }, { id: "520200", name: "六盘水市" },
    { id: "520300", name: "遵义市" }, { id: "520400", name: "安顺市" },
    { id: "520500", name: "毕节市" }, { id: "520600", name: "铜仁市" },
    { id: "522300", name: "黔西南布依族苗族自治州" }, { id: "522600", name: "黔东南苗族侗族自治州" },
    { id: "522700", name: "黔南布依族苗族自治州" }
];

AreaData.cities["530000"] = [
    { id: "530100", name: "昆明市" }, { id: "530300", name: "曲靖市" },
    { id: "530400", name: "玉溪市" }, { id: "530500", name: "保山市" },
    { id: "530600", name: "昭通市" }, { id: "530700", name: "丽江市" },
    { id: "530800", name: "普洱市" }, { id: "530900", name: "临沧市" },
    { id: "532300", name: "楚雄彝族自治州" }, { id: "532500", name: "红河哈尼族彝族自治州" },
    { id: "532600", name: "文山壮族苗族自治州" }, { id: "532800", name: "西双版纳傣族自治州" },
    { id: "532900", name: "大理白族自治州" }, { id: "533100", name: "德宏傣族景颇族自治州" },
    { id: "533300", name: "怒江傈僳族自治州" }, { id: "533400", name: "迪庆藏族自治州" }
];

AreaData.cities["540000"] = [
    { id: "540100", name: "拉萨市" }, { id: "540200", name: "日喀则市" },
    { id: "540300", name: "昌都市" }, { id: "540400", name: "林芝市" },
    { id: "540500", name: "山南市" }, { id: "540600", name: "那曲市" },
    { id: "542500", name: "阿里地区" }
];

AreaData.cities["610000"] = [
    { id: "610100", name: "西安市" }, { id: "610200", name: "铜川市" },
    { id: "610300", name: "宝鸡市" }, { id: "610400", name: "咸阳市" },
    { id: "610500", name: "渭南市" }, { id: "610600", name: "延安市" },
    { id: "610700", name: "汉中市" }, { id: "610800", name: "榆林市" },
    { id: "610900", name: "安康市" }, { id: "611000", name: "商洛市" }
];

AreaData.cities["620000"] = [
    { id: "620100", name: "兰州市" }, { id: "620200", name: "嘉峪关市" },
    { id: "620300", name: "金昌市" }, { id: "620400", name: "白银市" },
    { id: "620500", name: "天水市" }, { id: "620600", name: "武威市" },
    { id: "620700", name: "张掖市" }, { id: "620800", name: "平凉市" },
    { id: "620900", name: "酒泉市" }, { id: "621000", name: "庆阳市" },
    { id: "621100", name: "定西市" }, { id: "621200", name: "陇南市" },
    { id: "622900", name: "临夏回族自治州" }, { id: "623000", name: "甘南藏族自治州" }
];

AreaData.cities["630000"] = [
    { id: "630100", name: "西宁市" }, { id: "630200", name: "海东市" },
    { id: "632200", name: "海北藏族自治州" }, { id: "632300", name: "黄南藏族自治州" },
    { id: "632500", name: "海南藏族自治州" }, { id: "632600", name: "果洛藏族自治州" },
    { id: "632700", name: "玉树藏族自治州" }, { id: "632800", name: "海西蒙古族藏族自治州" }
];

AreaData.cities["640000"] = [
    { id: "640100", name: "银川市" }, { id: "640200", name: "石嘴山市" },
    { id: "640300", name: "吴忠市" }, { id: "640400", name: "固原市" },
    { id: "640500", name: "中卫市" }
];

AreaData.cities["650000"] = [
    { id: "650100", name: "乌鲁木齐市" }, { id: "650200", name: "克拉玛依市" },
    { id: "650400", name: "吐鲁番市" }, { id: "650500", name: "哈密市" },
    { id: "652300", name: "昌吉回族自治州" }, { id: "652700", name: "博尔塔拉蒙古自治州" },
    { id: "652800", name: "巴音郭楞蒙古自治州" }, { id: "652900", name: "阿克苏地区" },
    { id: "653000", name: "克孜勒苏柯尔克孜自治州" }, { id: "653100", name: "喀什地区" },
    { id: "653200", name: "和田地区" }, { id: "654000", name: "伊犁哈萨克自治州" },
    { id: "654200", name: "塔城地区" }, { id: "654300", name: "阿勒泰地区" },
    { id: "659001", name: "石河子市" }, { id: "659002", name: "阿拉尔市" },
    { id: "659003", name: "图木舒克市" }, { id: "659004", name: "五家渠市" },
    { id: "659005", name: "北屯市" }, { id: "659006", name: "铁门关市" },
    { id: "659007", name: "双河市" }, { id: "659008", name: "可克达拉市" },
    { id: "659009", name: "昆玉市" }
];

// 一些常用的区/县数据
AreaData.districts["110100"] = [
    { id: "110101", name: "东城区" }, { id: "110102", name: "西城区" },
    { id: "110105", name: "朝阳区" }, { id: "110106", name: "丰台区" },
    { id: "110107", name: "石景山区" }, { id: "110108", name: "海淀区" },
    { id: "110109", name: "门头沟区" }, { id: "110111", name: "房山区" },
    { id: "110112", name: "通州区" }, { id: "110113", name: "顺义区" },
    { id: "110114", name: "昌平区" }, { id: "110115", name: "大兴区" },
    { id: "110116", name: "怀柔区" }, { id: "110117", name: "平谷区" },
    { id: "110118", name: "密云区" }, { id: "110119", name: "延庆区" }
];

AreaData.districts["310100"] = [
    { id: "310101", name: "黄浦区" }, { id: "310104", name: "徐汇区" },
    { id: "310105", name: "长宁区" }, { id: "310106", name: "静安区" },
    { id: "310107", name: "普陀区" }, { id: "310109", name: "虹口区" },
    { id: "310110", name: "杨浦区" }, { id: "310112", name: "闵行区" },
    { id: "310113", name: "宝山区" }, { id: "310114", name: "嘉定区" },
    { id: "310115", name: "浦东新区" }, { id: "310116", name: "金山区" },
    { id: "310117", name: "松江区" }, { id: "310118", name: "青浦区" },
    { id: "310120", name: "奉贤区" }, { id: "310151", name: "崇明区" }
];

AreaData.districts["440100"] = [
    { id: "440103", name: "荔湾区" }, { id: "440104", name: "越秀区" },
    { id: "440105", name: "海珠区" }, { id: "440106", name: "天河区" },
    { id: "440111", name: "白云区" }, { id: "440112", name: "黄埔区" },
    { id: "440113", name: "番禺区" }, { id: "440114", name: "花都区" },
    { id: "440115", name: "南沙区" }, { id: "440117", name: "从化区" },
    { id: "440118", name: "增城区" }
];

AreaData.districts["440300"] = [
    { id: "440303", name: "罗湖区" }, { id: "440304", name: "福田区" },
    { id: "440305", name: "南山区" }, { id: "440306", name: "宝安区" },
    { id: "440307", name: "龙岗区" }, { id: "440308", name: "盐田区" },
    { id: "440309", name: "龙华区" }, { id: "440310", name: "坪山区" },
    { id: "440311", name: "光明区" }
];

AreaData.districts["320100"] = [
    { id: "320102", name: "玄武区" }, { id: "320104", name: "秦淮区" },
    { id: "320105", name: "建邺区" }, { id: "320106", name: "鼓楼区" },
    { id: "320111", name: "浦口区" }, { id: "320113", name: "栖霞区" },
    { id: "320114", name: "雨花台区" }, { id: "320115", name: "江宁区" },
    { id: "320116", name: "六合区" }, { id: "320117", name: "溧水区" },
    { id: "320118", name: "高淳区" }
];

AreaData.districts["330100"] = [
    { id: "330102", name: "上城区" }, { id: "330105", name: "拱墅区" },
    { id: "330106", name: "西湖区" }, { id: "330108", name: "滨江区" },
    { id: "330110", name: "余杭区" }, { id: "330111", name: "富阳区" },
    { id: "330112", name: "临安区" }, { id: "330113", name: "萧山区" },
    { id: "330114", name: "临平区" }, { id: "330122", name: "桐庐县" },
    { id: "330127", name: "淳安县" }
];

AreaData.districts["510100"] = [
    { id: "510104", name: "锦江区" }, { id: "510105", name: "青羊区" },
    { id: "510106", name: "金牛区" }, { id: "510107", name: "武侯区" },
    { id: "510108", name: "成华区" }, { id: "510112", name: "龙泉驿区" },
    { id: "510113", name: "青白江区" }, { id: "510114", name: "新都区" },
    { id: "510115", name: "温江区" }, { id: "510116", name: "双流区" },
    { id: "510117", name: "郫都区" }, { id: "510118", name: "新津区" },
    { id: "510129", name: "大邑县" }, { id: "510131", name: "蒲江县" },
    { id: "510181", name: "都江堰市" }, { id: "510182", name: "彭州市" },
    { id: "510183", name: "邛崃市" }, { id: "510184", name: "崇州市" },
    { id: "510185", name: "简阳市" }
];

AreaData.districts["610100"] = [
    { id: "610102", name: "新城区" }, { id: "610103", name: "碑林区" },
    { id: "610104", name: "莲湖区" }, { id: "610111", name: "灞桥区" },
    { id: "610112", name: "未央区" }, { id: "610113", name: "雁塔区" },
    { id: "610114", name: "阎良区" }, { id: "610115", name: "临潼区" },
    { id: "610116", name: "长安区" }, { id: "610117", name: "高陵区" },
    { id: "610118", name: "鄠邑区" }, { id: "610122", name: "蓝田县" },
    { id: "610124", name: "周至县" }
];

AreaData.districts["370100"] = [
    { id: "370102", name: "历下区" }, { id: "370103", name: "市中区" },
    { id: "370104", name: "槐荫区" }, { id: "370105", name: "天桥区" },
    { id: "370112", name: "历城区" }, { id: "370113", name: "长清区" },
    { id: "370114", name: "章丘区" }, { id: "370115", name: "济阳区" },
    { id: "370116", name: "莱芜区" }, { id: "370117", name: "钢城区" },
    { id: "370124", name: "平阴县" }, { id: "370126", name: "商河县" }
];

AreaData.districts["420100"] = [
    { id: "420102", name: "江岸区" }, { id: "420103", name: "江汉区" },
    { id: "420104", name: "硚口区" }, { id: "420105", name: "汉阳区" },
    { id: "420106", name: "武昌区" }, { id: "420107", name: "青山区" },
    { id: "420111", name: "洪山区" }, { id: "420112", name: "东西湖区" },
    { id: "420113", name: "汉南区" }, { id: "420114", name: "蔡甸区" },
    { id: "420115", name: "江夏区" }, { id: "420116", name: "黄陂区" },
    { id: "420117", name: "新洲区" }
];

AreaData.districts["370200"] = [
    { id: "370202", name: "市南区" }, { id: "370203", name: "市北区" },
    { id: "370211", name: "黄岛区" }, { id: "370212", name: "崂山区" },
    { id: "370213", name: "李沧区" }, { id: "370214", name: "城阳区" },
    { id: "370215", name: "即墨区" }, { id: "370281", name: "胶州市" },
    { id: "370282", name: "平度市" }, { id: "370283", name: "莱西市" }
];

// 默认街道数据（实际使用时可调用API获取完整数据）
AreaData.districts["default"] = [
    { id: "street001", name: "街道1" }, { id: "street002", name: "街道2" },
    { id: "street003", name: "街道3" }, { id: "street004", name: "街道4" }
];

// 初始化地址选择器
function initAddressSelector(options) {
    var selProvince = document.getElementById('selProvince');
    var selCity = document.getElementById('selCity');
    var selDistrict = document.getElementById('selDistrict');
    var selStreet = document.getElementById('selStreet');

    if (!selProvince) return;

    // 填充省份
    selProvince.innerHTML = '<option value="">请选择省</option>';
    AreaData.provinces.forEach(function(province) {
        var selected = (options && options.province === province.name) ? ' selected' : '';
        selProvince.innerHTML += '<option value="' + province.name + '" data-id="' + province.id + '"' + selected + '>' + province.name + '</option>';
    });

    // 省份选择事件
    selProvince.addEventListener('change', function() {
        var selected = this.options[this.selectedIndex];
        var provinceId = selected.getAttribute('data-id') || '';
        var cities = AreaData.cities[provinceId] || [];

        selCity.innerHTML = '<option value="">请选择市</option>';
        selDistrict.innerHTML = '<option value="">请选择区/县</option>';
        selStreet.innerHTML = '<option value="">请选择街道</option>';

        cities.forEach(function(city) {
            selCity.innerHTML += '<option value="' + city.name + '" data-id="' + city.id + '">' + city.name + '</option>';
        });
    });

    // 城市选择事件
    selCity.addEventListener('change', function() {
        var selected = this.options[this.selectedIndex];
        var cityId = selected.getAttribute('data-id') || '';
        var districts = AreaData.districts[cityId] || AreaData.districts['default'] || [];

        selDistrict.innerHTML = '<option value="">请选择区/县</option>';
        selStreet.innerHTML = '<option value="">请选择街道</option>';

        districts.forEach(function(district) {
            selDistrict.innerHTML += '<option value="' + district.name + '" data-id="' + district.id + '">' + district.name + '</option>';
        });
    });

    // 区/县选择事件
    selDistrict.addEventListener('change', function() {
        selStreet.innerHTML = '<option value="">请选择街道</option>';

        // 使用默认街道或提示用户
        var districts = AreaData.districts['default'];
        if (districts) {
            districts.forEach(function(street) {
                selStreet.innerHTML += '<option value="' + street.name + '">' + street.name + '</option>';
            });
        }
    });

    // 如果有初始值，触发选择
    if (options) {
        if (options.province) {
            selProvince.dispatchEvent(new Event('change'));
        }
        if (options.city) {
            setTimeout(function() {
                var cityOptions = selCity.options;
                for (var i = 0; i < cityOptions.length; i++) {
                    if (cityOptions[i].value === options.city) {
                        cityOptions[i].selected = true;
                        selCity.dispatchEvent(new Event('change'));
                        break;
                    }
                }
            }, 100);
        }
        if (options.district) {
            setTimeout(function() {
                var districtOptions = selDistrict.options;
                for (var i = 0; i < districtOptions.length; i++) {
                    if (districtOptions[i].value === options.district) {
                        districtOptions[i].selected = true;
                        break;
                    }
                }
            }, 200);
        }
        if (options.street) {
            setTimeout(function() {
                var streetOptions = selStreet.options;
                for (var i = 0; i < streetOptions.length; i++) {
                    if (streetOptions[i].value === options.street) {
                        streetOptions[i].selected = true;
                        break;
                    }
                }
            }, 300);
        }
    }
}
