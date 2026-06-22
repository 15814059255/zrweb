-- 阻容网 ZR.net.cn 数据库基线
-- Database: zr_platform
-- 说明：页面原型中的所有业务数据都应落在本库下，前端静态数字只作为样例展示。

CREATE DATABASE IF NOT EXISTS zr_platform
  DEFAULT CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE zr_platform;

CREATE TABLE members (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  member_no VARCHAR(32) NOT NULL UNIQUE,
  company_name VARCHAR(120) NOT NULL,
  short_name VARCHAR(80),
  contact_name VARCHAR(40),
  phone VARCHAR(40),
  qq VARCHAR(40),
  email VARCHAR(120),
  address VARCHAR(255),
  province VARCHAR(40),
  city VARCHAR(40),
  district VARCHAR(40),
  street VARCHAR(80),
  detail_address VARCHAR(255),
  role ENUM('采购商','供应商','供应商 + 采购商') NOT NULL,
  status ENUM('启用','禁用') NOT NULL DEFAULT '启用',
  registered_at DATETIME NOT NULL,
  last_login_at DATETIME,
  last_login_ip VARCHAR(64),
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  INDEX idx_members_company (company_name),
  INDEX idx_members_role_status (role, status)
);

CREATE TABLE member_profiles (
  member_id BIGINT PRIMARY KEY,
  city VARCHAR(80),
  position_name VARCHAR(80),
  intro TEXT,
  categories VARCHAR(255),
  models TEXT,
  trade_mode VARCHAR(80),
  delivery_mode VARCHAR(80),
  tax_support VARCHAR(80),
  license_status ENUM('未上传','已上传','已审核') DEFAULT '未上传',
  office_photo_status ENUM('未上传','已上传','已审核') DEFAULT '未上传',
  warehouse_photo_status ENUM('未上传','已上传','已审核') DEFAULT '未上传',
  auth_doc_status ENUM('未上传','已上传','已审核') DEFAULT '未上传',
  updated_at DATETIME NOT NULL
);

CREATE TABLE brands (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  brand_name VARCHAR(80) NOT NULL UNIQUE,
  brand_alias VARCHAR(120),
  category_scope VARCHAR(120),
  status ENUM('启用','禁用') NOT NULL DEFAULT '启用',
  sort_order INT NOT NULL DEFAULT 0,
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  INDEX idx_brands_status_sort (status, sort_order)
);

CREATE TABLE member_brands (
  member_id BIGINT NOT NULL,
  brand_id BIGINT NOT NULL,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (member_id, brand_id)
);

INSERT INTO brands(brand_name, brand_alias, category_scope, status, sort_order, created_at, updated_at) VALUES
('Murata', '村田', '电容,电阻', '启用', 10, NOW(), NOW()),
('Yageo', '国巨', '电阻,电容', '启用', 20, NOW(), NOW()),
('Samsung', '三星电机', '电容', '启用', 30, NOW(), NOW()),
('TDK', 'TDK', '电容', '启用', 40, NOW(), NOW()),
('Vishay', '威世', '电阻', '启用', 50, NOW(), NOW()),
('风华高科', 'FH', '电阻,电容', '启用', 60, NOW(), NOW());

CREATE TABLE supply_items (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  member_id BIGINT NOT NULL,
  model VARCHAR(120) NOT NULL,
  brand VARCHAR(80),
  package_name VARCHAR(40),
  params_text VARCHAR(255),
  qty INT NOT NULL,
  unit VARCHAR(20) NOT NULL,
  price DECIMAL(18,4) NOT NULL,
  tax_type ENUM('含税','未税') NOT NULL,
  batch_no VARCHAR(40),
  valid_until DATETIME,
  status ENUM('在线','已下架','已过期') NOT NULL DEFAULT '在线',
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  INDEX idx_supply_model (model),
  INDEX idx_supply_member_status (member_id, status),
  INDEX idx_supply_created (created_at)
);

CREATE TABLE demand_items (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  member_id BIGINT NOT NULL,
  model VARCHAR(120) NOT NULL,
  brand_preference VARCHAR(120),
  params_text VARCHAR(255),
  qty INT NOT NULL,
  unit VARCHAR(20) NOT NULL,
  expected_price DECIMAL(18,4) NULL,
  tax_type ENUM('含税','未税','面议') NOT NULL DEFAULT '面议',
  valid_until DATETIME,
  status ENUM('在线','已下架','已过期') NOT NULL DEFAULT '在线',
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  INDEX idx_demand_model (model),
  INDEX idx_demand_member_status (member_id, status),
  INDEX idx_demand_created (created_at)
);

CREATE TABLE inquiries (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  supply_id BIGINT NOT NULL,
  buyer_id BIGINT NOT NULL,
  supplier_id BIGINT NOT NULL,
  qty INT NOT NULL,
  unit VARCHAR(20) NOT NULL,
  target_price DECIMAL(18,4),
  tax_type ENUM('含税','未税','面议') NOT NULL DEFAULT '面议',
  message VARCHAR(500),
  status ENUM('新询价','已报价','已关闭') NOT NULL DEFAULT '新询价',
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  INDEX idx_inquiries_supplier_status (supplier_id, status),
  INDEX idx_inquiries_buyer (buyer_id)
);

CREATE TABLE quotes (
  id BIGINT PRIMARY KEY AUTO_INCREMENT,
  demand_id BIGINT NOT NULL,
  supplier_id BIGINT NOT NULL,
  buyer_id BIGINT NOT NULL,
  model VARCHAR(120) NOT NULL,
  qty INT NOT NULL,
  unit VARCHAR(20) NOT NULL,
  price DECIMAL(18,4) NOT NULL,
  tax_type ENUM('含税','未税') NOT NULL,
  batch_no VARCHAR(40),
  expire_days INT,
  status ENUM('新报价','推荐','普通','已过期') NOT NULL DEFAULT '新报价',
  created_at DATETIME NOT NULL,
  INDEX idx_quote_model (model),
  INDEX idx_quote_supplier (supplier_id),
  INDEX idx_quote_buyer_created (buyer_id, created_at)
);

-- 采购工作台统计查询
-- SELECT COUNT(*) FROM demand_items WHERE member_id = :current_member AND status = '在线';
-- SELECT COUNT(*) FROM quotes WHERE buyer_id = :current_member;
-- SELECT COUNT(*) FROM quotes WHERE buyer_id = :current_member AND status = '新报价';
-- SELECT COUNT(*) FROM demand_items WHERE member_id = :current_member AND status IN ('已下架','已过期');

CREATE TABLE system_settings (
  setting_key VARCHAR(80) PRIMARY KEY,
  setting_value VARCHAR(255) NOT NULL,
  updated_at DATETIME NOT NULL
);

INSERT INTO system_settings(setting_key, setting_value, updated_at)
VALUES ('default_member_role', '采购商', NOW())
ON DUPLICATE KEY UPDATE setting_value = VALUES(setting_value), updated_at = VALUES(updated_at);

-- 后台 KPI 对应查询
-- 会员总数
-- SELECT COUNT(*) FROM members;
-- 在线供应
-- SELECT COUNT(*) FROM supply_items WHERE status = '在线';
-- 在线求购
-- SELECT COUNT(*) FROM demand_items WHERE status = '在线';
-- 今日报价
-- SELECT COUNT(*) FROM quotes WHERE DATE(created_at) = CURRENT_DATE;
