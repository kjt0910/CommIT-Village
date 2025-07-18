CREATE TABLE tasks(
    id BIGSERIAL PRIMARY KEY,
    attendance_id INT NOT NULL,
    task_text TEXT,
    start_task_at TIMESTAMP WITH TIME ZONE NOT NULL,
    end_task_at TIMESTAMP WITH TIME ZONE,
    FOREIGN KEY (attendance_id) REFERENCES attendance(id)
);

COMMENT ON TABLE tasks IS '作業履歴テーブル';
COMMENT ON COLUMN tasks.id IS '作業ID(主キー)';
COMMENT ON COLUMN tasks.task_text IS '作業内容詳細';
COMMENT ON COLUMN tasks.start_task_at IS '作業開始日時';
COMMENT ON COLUMN tasks.end_task_at IS '作業終了日時';
