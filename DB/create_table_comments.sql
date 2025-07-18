CREATE TABLE comments(
    id BIGSERIAL PRIMARY KEY,
    question_id INT NOT NULL,
    user_id INT NOT NULL,
    comment_text TEXT NOT NULL,
    comment_created_at TIMESTAMP WITH TIME ZONE NOT NULL,
    FOREIGN KEY (question_id) REFERENCES questions(id),
    FOREIGN KEY (user_id) REFERENCES users(id)
);

COMMENT ON TABLE comments IS 'コメント管理テーブル';
COMMENT ON COLUMN comments.id IS 'コメントID(主キー)';
COMMENT ON COLUMN comments.question_id IS '対象質問のID';
COMMENT ON COLUMN comments.user_id IS 'コメント投稿者のユーザID';
COMMENT ON COLUMN comments.comment_text IS 'コメント内容';
COMMENT ON COLUMN comments.comment_created_at IS 'コメント作成日時';