CREATE TABLE users(
    id BIGSERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    birth_date DATE,
    role user_role DEFAULT'一般',
    phone VARCHAR(20),
    email VARCHAR(255),
    password VARCHAR(255)
);

COMMENT ON TABLE users IS 'ユーザ情報を管理するテーブル';
COMMENT ON COLUMN users.id IS 'ユーザID(主キー)';
COMMENT ON COLUMN users.name IS '氏名';
COMMENT ON COLUMN users.birth_date IS '生年月日';
COMMENT ON COLUMN users.role IS '役職';
COMMENT ON COLUMN users.phone IS '電話番号';
COMMENT ON COLUMN users.email IS 'メールアドレス';
COMMENT ON COLUMN users.password IS 'パスワード';