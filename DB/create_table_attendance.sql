CREATE TABLE attendance(
    id BIGSERIAL PRIMARY KEY,
    user_id INT NOT NULL,
    attendance_date DATE NOT NULL,
    clock_in_time TIME,
    clock_out_time TIME,
    work_type work_type NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL,
    FOREIGN KEY(user_id) REFERENCES users(id),
    UNIQUE(user_id, attendance_date)    ---ユニーク制約で同ユーザが同日に複数勤怠レコードを登録できないようにする
);

COMMENT ON TABLE attendance IS '退勤管理テーブル';
COMMENT ON COLUMN attendance.id IS '勤怠ID(主キー)';
COMMENT ON COLUMN attendance.user_id IS 'ユーザID(外部キー)';
COMMENT ON COLUMN attendance.attendance_date IS '勤務日';
COMMENT ON COLUMN attendance.clock_in_time IS '出勤時刻';
COMMENT ON COLUMN attendance.clock_out_time IS '退勤時刻';
COMMENT ON COLUMN attendance.work_type IS '勤務形態（出勤orリモート）';
COMMENT ON COLUMN attendance.created_at IS '記録作成日時';
COMMENT ON COLUMN attendance.updated_at IS '記録更新日時';