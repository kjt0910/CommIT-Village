CREATE TABLE holidays(
    id BIGSERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    holiday_date DATE NOT NULL,
    is_holiday BOOLEAN NOT NULL DEFAULT TRUE,
    holiday_created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    holiday_updated_at TIMESTAMP WITH TIME ZONE NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    UNIQUE(user_id, holiday_date)
);

COMMENT ON TABLE holidays IS '休日管理テーブル';
COMMENT ON COLUMN holidays.id IS'登録ID(主キー)';
COMMENT ON COLUMN holidays.user_id IS '対象者のユーザID';
COMMENT ON COLUMN holidays.holiday_date IS '対象の日付';
COMMENT ON COLUMN holidays.is_holiday IS '休日フラグ(TRUE:休日, FALSE:勤務日)';
COMMENT ON COLUMN holidays.holiday_created_at IS '登録日時';
COMMENT ON COLUMN holidays.holiday_updated_at IS '更新日時';
